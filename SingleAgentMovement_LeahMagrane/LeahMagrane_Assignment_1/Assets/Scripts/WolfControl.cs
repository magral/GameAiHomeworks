using UnityEngine;
using System.Collections;

public class WolfControl : MonoBehaviour {


	private Rigidbody2D rb_;
	private float acceleration, maxAcceleration;
	void Awake()
	{
		acceleration = .5f;
		maxAcceleration = 1f;
		rb_ = GetComponent<Rigidbody2D>();
	}

	void Update () {
		MovementFunctions.AccelerateClamped(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), rb_, acceleration, maxAcceleration);
	}
}
