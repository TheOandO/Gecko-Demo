using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEye : MonoBehaviour
{
	Animator animator;
	Rigidbody2D rb;
	Damagable dmgable;

	public float flightSpeed = 2f;
	public float waypointReachedDistance = 0.1f;
	public DetectionZone biteDetectZone;
	public Collider2D deathCollider;
	public List<Transform> waypoints;

	Transform nextWaypoint;
	int waypointNum = 0;
	Transform player;

	Vector2 initialPosition;

	public bool _hasTarget = false;

	public bool HasTarget
	{
		get
		{
			return _hasTarget;
		}
		private set
		{
			_hasTarget = value;
			animator.SetBool(AnimationStrings.hasTarget, value);
		}
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		dmgable = GetComponent<Damagable>();
		player = GameObject.FindGameObjectWithTag("Player").transform;

		initialPosition = transform.position;
	}

	private void OnEnable()
	{
		dmgable.dmgableDeath.AddListener(OnDeath);
	}

	private void Start()
	{
		nextWaypoint = waypoints[waypointNum];
	}

	void Update()
	{
		HasTarget = biteDetectZone.detectedColl.Count > 0;
	}

	private void FixedUpdate()
	{
		if (dmgable.IsAlive)
		{
			if (CanMove)
			{
				Flight();
			}
			else
			{
				rb.velocity = Vector3.zero;
			}
		}
	}

	public bool CanMove
	{
		get
		{
			return animator.GetBool(AnimationStrings.canMove);
		}
	}

	private void Flight()
	{
		//Fly to waypoint
		Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

		float distance = Vector2.Distance(nextWaypoint.position, transform.position);

		rb.velocity = directionToWaypoint * flightSpeed;

		if (waypoints.Count <= 2 && waypoints[0].position.y != waypoints[1].position.y)
		{
			UpdateDirectionTowardsPlayer();
		}
		else
		{
			UpdateDirection();
		}

		if (distance <= waypointReachedDistance)
		{
			waypointNum++;

			if (waypointNum >= waypoints.Count)
			{
				waypointNum = 0;
			}

			nextWaypoint = waypoints[waypointNum];
		}
	}

	private void UpdateDirection()
	{
		Vector3 locScale = transform.localScale;

		if (transform.localScale.x > 0)
		{
			if (rb.velocity.x < 0)
			{
				transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
			}
		}
		else
		{
			if (rb.velocity.x > 0)
			{
				transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
			}
		}
	}

	private void UpdateDirectionTowardsPlayer()
	{
		Vector3 locScale = transform.localScale;

		if (player != null)
		{
			if (player.position.x > transform.position.x)
			{
				transform.localScale = new Vector3(Mathf.Abs(locScale.x), locScale.y, locScale.z);
			}
			else if (player.position.x < transform.position.x)
			{
				transform.localScale = new Vector3(-Mathf.Abs(locScale.x), locScale.y, locScale.z);
			}
		}
	}

	public void OnDeath()
	{
		rb.gravityScale = 2f;
		rb.velocity = new Vector2(0, rb.velocity.y);
		deathCollider.enabled = true;
	}

	public void ResetFlyEye()
	{
		transform.position = initialPosition;

		rb.gravityScale = 0;
		rb.velocity = Vector2.zero;
		waypointNum = 0;
		nextWaypoint = waypoints[waypointNum];

		dmgable.IsAlive = true;
		animator.SetBool(AnimationStrings.canMove, true);
	}
}

