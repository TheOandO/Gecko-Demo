using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
	Animator animator;
	Rigidbody2D rb;

	public float flightSpeed = 2f;
	public float waypointReachedDistance = 0.1f;
	public List<Transform> waypoints;
	public float arcHeight = 2f; // Controls the arc shape

	private Transform nextWaypoint;
	private int waypointNum = 0;
	private float t = 0; // Bezier curve progress
	private bool isFlying = false; // Determines if bird is moving

	private Vector2 startPosition;
	private Vector2 controlPoint;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		nextWaypoint = waypoints[waypointNum];
		SetFlightPath();
	}

	private void FixedUpdate()
	{
		if (isFlying)
		{
			FollowBezierCurve();
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
	}

	/// <summary>
	/// Manually start flying.
	/// </summary>
	public void StartFlying()
	{
		if (!isFlying)
		{
			isFlying = true;
			animator.SetBool("isFlying", true); // Activate flying animation
		}
	}

	/// <summary>
	/// Stops the bird when reaching a waypoint.
	/// </summary>
	private void StopFlying()
	{
		isFlying = false;
		animator.SetBool("isFlying", false); // Switch to idle animation
	}

	private void SetFlightPath()
	{
		startPosition = transform.position;
		nextWaypoint = waypoints[waypointNum];

		// Calculate control point for arc movement
		Vector2 midpoint = (startPosition + (Vector2)nextWaypoint.position) / 2;
		controlPoint = new Vector2(midpoint.x, midpoint.y + arcHeight);

		t = 0; // Reset curve interpolation
	}

	private void FollowBezierCurve()
	{
		if (t < 1)
		{
			t += Time.fixedDeltaTime * flightSpeed / Vector2.Distance(startPosition, nextWaypoint.position);

			// Quadratic Bezier formula
			Vector2 newPos = Mathf.Pow(1 - t, 2) * startPosition +
							 2 * (1 - t) * t * controlPoint +
							 Mathf.Pow(t, 2) * (Vector2)nextWaypoint.position;

			rb.MovePosition(newPos);
			UpdateDirection(newPos);
		}
		else
		{
			StopFlying(); // Stop bird when reaching waypoint
			waypointNum = (waypointNum + 1) % waypoints.Count;
			SetFlightPath();
		}
	}

	private void UpdateDirection(Vector2 newPosition)
	{
		if (newPosition.x > transform.position.x)
		{
			transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
		}
		else
		{
			transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
		}
	}
}
