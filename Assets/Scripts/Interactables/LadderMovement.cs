using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LadderMovement : MonoBehaviour
{
	[Header("LadderMovement")]
	private float verticalMove;
	private float climbLadderSpeed = 4f;
	public bool isClimbingLadder;

	private Rigidbody2D rb;
	[SerializeField]private LayerMask whatIsLadder;
	
	public float distance;

	private PlayerInput playerInput;
	private InputAction moveAction;

	void Start()
	{
		rb = GetComponentInParent<Rigidbody2D>(); 
		playerInput = GetComponentInParent<PlayerInput>();

		moveAction = playerInput.actions["VerticalMove"];
	}

	void Update()
	{
		RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.up, distance, whatIsLadder);

		if (hitInfo.collider != null)
		{
			if (moveAction.ReadValue<Vector2>().y > 0)
			{
				isClimbingLadder = true;
			}
		}
		else
		{
			isClimbingLadder = false;
		}

		if (isClimbingLadder)
		{
			verticalMove = moveAction.ReadValue<Vector2>().y;

		}
	}

	private void FixedUpdate()
	{
		if (isClimbingLadder)
		{
			rb.gravityScale = 0;
			rb.velocity = new Vector2(rb.velocity.x, verticalMove * climbLadderSpeed);
		} else
		{
			rb.gravityScale = 1f;
		}
	}
}
