using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetect : MonoBehaviour
{
	[SerializeField] private float radius;
	[SerializeField] private LayerMask whereGround;
	[SerializeField] private TouchDirections playr;
	[SerializeField] private float ledgeCooldownDuration = 0.1f;

	private bool canDetect;
	private float ledgeCooldownTimer = 0f;
	private void Update()
	{
		if (ledgeCooldownTimer > 0)
		{
			ledgeCooldownTimer -= Time.deltaTime;
		}

		if (canDetect && ledgeCooldownTimer <= 0f)
			playr.ledgeDetected = Physics2D.OverlapCircle(transform.position, radius, whereGround);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			canDetect = false;
			ledgeCooldownTimer = ledgeCooldownDuration;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && ledgeCooldownTimer <= 0f)
			canDetect = true;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
