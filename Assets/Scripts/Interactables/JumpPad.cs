using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
	Animator animator;
	PlayerController playerController;

	public float bounce = 11f;
	public float extraUpwardForce = 5f;

	void Start()
	{
		playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

		animator = GetComponent<Animator>();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			animator.SetTrigger("activated");

			Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

			Vector2 bounceDirection = transform.up;
			playerRb.AddForce(bounceDirection * bounce, ForceMode2D.Impulse);

			playerRb.AddForce(Vector2.up * extraUpwardForce, ForceMode2D.Impulse);

			playerController.isOnJumpPad = true;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			playerController.isOnJumpPad = false;
		}
	}
}
