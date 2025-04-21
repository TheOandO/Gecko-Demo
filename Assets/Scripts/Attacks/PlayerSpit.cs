using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpit : MonoBehaviour
{
	public int dmg = 10;
	public float force;
	public float bounceForce = 5f;
	public float destroyTime = 5;
	public Vector2 knockback = new Vector2(0, 0);

	private bool hasBounced = false;

	[SerializeField] private float angleOffset = 45f;

	private float timer;

	Rigidbody2D rb;
	Animator animator;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
	}

	void Start()
	{
		Vector2 diagonalDirection = GetDiagonalDirection();

		if (transform.localScale.x < 0)
		{
			diagonalDirection.x = -diagonalDirection.x;
		}

		rb.velocity = diagonalDirection * force;

		float rotation = Mathf.Atan2(diagonalDirection.y, diagonalDirection.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0, 0, rotation);
	}

	private void FixedUpdate()
	{
		timer += Time.deltaTime;

		if (timer > destroyTime)
		{
			DestroyProjectile();
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Damagable dmgable = collision.collider.GetComponent<Damagable>();

		if (dmgable != null)
		{
			Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);
			bool gitHit = dmgable.Hit(dmg, deliveredKnockback);

			if (gitHit)
			{
				ExplodeProjectile();
			}
		}
		else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			ProjectileBounce(collision);
		}
	}

	private Vector2 GetDiagonalDirection()
	{
		float angleInRadians = angleOffset * Mathf.Deg2Rad;

		return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;
	}

	private void ProjectileBounce(Collision2D collision)
	{
		if (!hasBounced)
		{
			Vector2 normal = collision.contacts[0].normal;
			if (Mathf.Abs(normal.y) > Mathf.Abs(normal.x))
			{
				rb.velocity = new Vector2(rb.velocity.x, bounceForce);
			}
			else
			{
				Vector2 reflectDir = Vector2.Reflect(rb.velocity.normalized, normal); // Calculate the reflection direction
				if (reflectDir.x != 0)
				{
					rb.velocity = reflectDir * bounceForce;
				}
				else
				{
					rb.velocity = new Vector2(rb.velocity.x - bounceForce * 0.7f, rb.velocity.y);
				}

			}
			Debug.Log(rb.velocity);
			hasBounced = true;
		}
		else
		{
			ExplodeProjectile();
		}
	}

	private void ExplodeProjectile()
	{
		AudioManager.Instance.PlaySFX(AudioManager.Instance.spitBallExplode);
		rb.velocity = Vector2.zero;
		rb.gravityScale = 0; 
		rb.angularVelocity = 0;
		animator.SetTrigger("explode");
	}

	public void DestroyProjectile()
	{
		Destroy(gameObject);
	}
}
