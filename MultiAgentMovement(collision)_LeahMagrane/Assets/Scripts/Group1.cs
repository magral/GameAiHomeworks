using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Group1 : MonoBehaviour {

	[SerializeField]
	private bool _ConeCollision;
	[SerializeField]
	private bool _CollisionPrediction;
	[SerializeField]
	private float _maxVelocity;
	[SerializeField]
	private float _acceleration;
	[SerializeField]
	private float _maxAcceleration;
	[SerializeField]
	private Rigidbody2D _targetRB;
	private float _slowRadius;
	private List<Vector2> _path;
	private int _priority;
	private int _index;
	private State _state;
	private Rigidbody2D _rb;

	private enum State{
		pathFollow,
		evade
	}

	void Awake()
	{
		_slowRadius = 2f;
		_index = 0;
		_rb = GetComponent<Rigidbody2D>();
		_state = State.pathFollow;
		_path = new List<Vector2>();
		_path.Add(new Vector2(-2.25f, 4.5f));
		_path.Add(new Vector2(-1.5f, -4.5f));
		_path.Add(new Vector2(0f, 4.5f));
		_path.Add(new Vector2(3.3f, -4.5f));
		_path.Add(new Vector2(6.75f, 4.5f));
	}

	void Update()
	{
		if (_state == State.pathFollow)
		{
			if (_index < _path.Count)
				_index = PathFollow(_path, _index, _rb, _acceleration, _maxAcceleration, _slowRadius, _maxVelocity);
			else if (_index >= _path.Count)
			{
				_index = 0;
			}
		}
		//Do ConeCollision
		if (_ConeCollision && ConeCollision(_targetRB, _rb, 30f, _maxVelocity))
		{
			TransitionState(State.evade);
			
		}
		else if (_ConeCollision && !ConeCollision(_targetRB, _rb, 30f, _maxVelocity))
		{
			TransitionState(State.pathFollow);
		}
		//Do Collision Prediction
		if (_CollisionPrediction && CollisionPrediction(_targetRB, _rb)) 
		{
			TransitionState(State.evade);
		}
		else if(_CollisionPrediction && !CollisionPrediction(_targetRB, _rb))
		{
			TransitionState(State.pathFollow);
		}

	}

	void TransitionState(State state)
	{
		_state = state;
		if(_state == State.evade)
		{
			_rb.velocity = new Vector2(0, 0);
		}
	}
	int PathFollow(List<Vector2> path, int index, Rigidbody2D rb, float acceleration, float maxAccel, float slowRadius, float maxVelocity)
	{
		Vector2 targetpos = path[index];
		Vector2 targetDis = targetpos - rb.position;

		float angle = Mathf.Atan2(targetDis.x, targetDis.y) * Mathf.Rad2Deg;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, q, Time.deltaTime * 2);

		targetDis = targetDis.normalized * Mathf.Min(targetDis.magnitude / slowRadius, maxVelocity);
		float dis = Mathf.Abs(Vector2.Distance(rb.position, targetpos));

		Vector2 dv = targetDis - rb.velocity;
		dv = dv.normalized * Mathf.Min(dv.magnitude * acceleration, maxAccel);
		rb.AddForce(dv * rb.mass, ForceMode2D.Force);

		if (dis <= .05f)
		{
			index++;
		}
		return index;
	}

	bool ConeCollision(Rigidbody2D target, Rigidbody2D character, float halfAngle, float maxVelocity)
	{
		//orientation + position (dot) the distance between the character and target position 
		if ((dotProduct(new Vector2(Mathf.Cos(character.transform.rotation.eulerAngles.z), Mathf.Sin(character.transform.rotation.eulerAngles.z)) + 
			new Vector2(character.transform.position.x, character.transform.position.y), 
			(target.transform.position - character.transform.position).normalized)) > Mathf.Cos(halfAngle))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	bool CollisionPrediction(Rigidbody2D target, Rigidbody2D character)
	{
		Vector2 dp = target.position - character.position;
		Vector2 dv = target.velocity - character.velocity;
		float t = -(dotProduct(dp, dv) / (Mathf.Pow(lengthV2(dp), 2)));
		Vector2 pc = (character.position + character.velocity) * t;
		Vector2 pt = (target.position + target.velocity) * t;
		if(Mathf.Abs(Vector2.Distance(pt, pc )) <= .035f)// && Vector2.Distance(pt, pc) <= .04f)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	float dotProduct(Vector2 a, Vector2 b)
	{
		return a.x * b.x + a.y * b.y;
	}

	float lengthV2(Vector2 v)
	{
		return Mathf.Sqrt(v.x * v.x + v.y * v.y);
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawRay(this.transform.position, transform.right);
		Gizmos.DrawRay(this.transform.position, _targetRB.position - (Vector2)this.transform.position);
	}
	
}
