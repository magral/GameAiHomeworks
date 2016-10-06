using UnityEngine;
using System.Collections.Generic;

public class Group2 : MonoBehaviour {

	[SerializeField]
	private float _maxVelocity;
	[SerializeField]
	private float _acceleration;
	[SerializeField]
	private float _maxAcceleration;

	private List<Vector2> _path;
	private float _slowRadius;
	private Rigidbody2D _rb;
	private int _index;

	void Awake()
	{
		_slowRadius = 2f;
		_rb = GetComponent<Rigidbody2D>();
		_path = new List<Vector2>();
		_path.Add(new Vector2(-2.25f, 4.5f));
		_path.Add(new Vector2(-1.5f, -4.5f));
		_path.Add(new Vector2(0f, 4.5f));
		_path.Add(new Vector2(3.3f, -4.5f));
		_path.Add(new Vector2(6.75f, 4.5f));
	}

	void Update()
	{
		if (_index < _path.Count)
			_index = PathFollow(_path, _index, _rb, _acceleration, _maxAcceleration, _slowRadius, _maxVelocity);
		else if (_index >= _path.Count)
		{
			_index = 0;
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
}
