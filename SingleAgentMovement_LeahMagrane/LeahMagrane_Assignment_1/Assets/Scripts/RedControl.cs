using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RedControl : MonoBehaviour {

	[Header("Pick which algorithm to test")]
	public bool Pursue;
    public bool dynamicWander;
	public bool flee;
	public bool pathFollow;
	[Header("Movement options")]
	public Rigidbody2D target;
	public float maxAcceleration;
	public float acceleration;
	public float maxVelocity;

	private float slowRadius;
	private Rigidbody2D rb_;
	private int start_;
	private List<Vector2> Path;
	private LineRenderer line_;

	// Use this for initialization
	void Awake () {
		rb_ = GetComponent<Rigidbody2D>();
		start_ = 0;
		slowRadius = 3f;
		line_ = GetComponent<LineRenderer>();
		Path = new List<Vector2>();
		Path.Add(new Vector2(.646f, -.438f));
		Path.Add(new Vector2(-.656f, -.404f));
		Path.Add(new Vector2(.568f, .065f));
		Path.Add(new Vector2(-.531f, .251f));
		Path.Add(new Vector2(.495f, .362f));
	}
	
	// Update is called once per frame
	void Update () {

		if (Pursue)
		{
			MovementFunctions.DynamicPursue(target, rb_, maxVelocity, acceleration, maxAcceleration, slowRadius);
		}

		else if (dynamicWander)
		{
			MovementFunctions.DynamicWander(rb_, acceleration, maxAcceleration);
		}
		else if (flee)
		{
			MovementFunctions.Evade(target, rb_, acceleration, maxAcceleration);
		}
		else if (pathFollow)
		{
			if (start_ < Path.Count)
			{
				start_ = MovementFunctions.PathFollow(rb_, Path, start_, maxVelocity, acceleration, maxAcceleration, slowRadius, line_);
			}
			else if(start_ >= Path.Count)
			{
				start_ = 0;
			}
		}
		else
		{
			MovementFunctions.AccelerateClamped(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), rb_, acceleration, maxAcceleration);
		}
	}

	public void FixedUpdate()
	{
		MovementFunctions.ClampVelocity(rb_, maxVelocity);
	}
}
