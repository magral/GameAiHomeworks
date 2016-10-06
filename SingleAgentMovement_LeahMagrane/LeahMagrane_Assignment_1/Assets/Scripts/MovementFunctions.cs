using UnityEngine;
using System.Collections.Generic;

public class MovementFunctions : MonoBehaviour {

    //function to actually move the body
	public static void AccelerateClamped(Vector2 target, Rigidbody2D rb, float acceleration, float maxAccel)
	{
		//Difference between desired velocity and current
		Vector2 dv = target - rb.velocity;
		dv = dv.normalized * Mathf.Min(dv.magnitude * acceleration, maxAccel);
		//Apply force to the body
		rb.AddForce(dv * rb.mass, ForceMode2D.Force);
	}

	public static float DynamicArrive(Vector2 targetpos, Rigidbody2D rb, float maxVelocity, float acceleration, float maxAccel, float slowRadius)
	{
		//Target is a vector that is the difference from the current position to the target position
		Vector2 targetDis = targetpos - rb.position;

		//Rotate orientation
		float angle = Mathf.Atan2(targetDis.x, targetDis.y) * Mathf.Rad2Deg;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, q, Time.deltaTime * 2);

		//Deal with slow Radius
		targetDis = targetDis.normalized * Mathf.Min(targetDis.magnitude / slowRadius, maxVelocity);

		AccelerateClamped(targetDis, rb, acceleration, maxAccel);
		float dis = Mathf.Abs(Vector2.Distance(rb.position, targetpos));
		return dis;
	}

	public static void DynamicPursue(Rigidbody2D targetPos, Rigidbody2D playerRB, float maxVelocity, float acceleration, float maxAccel, float slowRadius)
	{
		//Predict ahead for the target's position
		Vector2 position = targetPos.position + targetPos.velocity * 2;

		DynamicArrive(position, playerRB, maxVelocity, acceleration, maxAccel, slowRadius);
		
	}

	public static void Flee(Vector2 targetPosition, Rigidbody2D rb, float acceleration, float maxAccel)
	{
		if (Vector2.Distance(targetPosition, rb.position) < 0.5f)
		{
			//Difference between desired velocity and current
			Vector2 dv = rb.position - targetPosition;
			//Set dv to the speed at which we want to go. The min of it's current magnitude * acceleration or the max acceleration
			dv = dv.normalized * Mathf.Min(dv.magnitude * acceleration, maxAccel);

			//Apply force to the body
			rb.AddForce(dv * rb.mass, ForceMode2D.Force);
		}
		else
		{
			rb.velocity = rb.velocity * .9f;
		}
	}

	public static void Evade(Rigidbody2D evading, Rigidbody2D evader, float acceleration, float maxAccel)
	{
		//Predict the movement of the other object to evade it
		Vector2 evadePosition = evading.position + evading.velocity * 3;
		Flee(evadePosition, evader, acceleration, maxAccel);
	}

	public static void DynamicWander(Rigidbody2D rb, float acceleration, float maxAccel)
	{
        //Change this to change the radius of the circle
        const int RADIUS = 8;
        //Pick a point in an arc in front of the object
        Transform transform = rb.GetComponent<Transform>();
        //Get the rotation of the object
        Quaternion rotation = transform.rotation;
        //Generate a random point on a circle around the offset
        float theta = Random.Range(0, 2 * Mathf.PI);
        float x = RADIUS * Mathf.Cos(theta) + rotation.x;
        float y = RADIUS * Mathf.Sin(theta) + rotation.y;
        //Create a vector from those points and accelerate towards it
        Vector2 orientation = new  Vector2(x, y);
		float angle = Mathf.Atan2(orientation.x, orientation.y) * Mathf.Rad2Deg;
		Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
		rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, q, Time.deltaTime * .5f);
		AccelerateClamped(orientation, rb, acceleration, maxAccel);
	}

	public static int PathFollow(Rigidbody2D character, List<Vector2> Path, int index, float maxVelocity, float accel, float maxAccel, float slowRadius, LineRenderer line)
	{
		float distance = DynamicArrive(Path[index], character, maxVelocity, accel, maxAccel, slowRadius);
		line.SetPosition(0, new Vector3(character.position.x, character.position.y, 0));
		line.SetPosition(1, new Vector3(Path[index].x, Path[index].y, 0));
		if(distance <= .05f)
		{
			index++;
		}
		return index;
	}
	public static void ClampVelocity(Rigidbody2D rb, float maxVelocity)
	{
		rb.velocity = rb.velocity.normalized * Mathf.Min(rb.velocity.magnitude, maxVelocity);
	}
}
