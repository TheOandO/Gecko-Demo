using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchSlowBall : MonoBehaviour
{
	public int dmg = 10;
	public float initialForce = 10f;
	public float destroyTime = 5f;
	[Range(0.9f, 1f)]
	public float slowdownRate = 0.95f;
	public Vector2 knockback = new Vector2(0, 0);

	private GameObject player;
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
		player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			Vector3 dir = player.transform.position - transform.position;
			rb.velocity = new Vector2(dir.x, dir.y).normalized * initialForce;

			float rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0, 0, rotation);
		}
	}

	private void FixedUpdate()
	{
		timer += Time.deltaTime;

		rb.velocity *= slowdownRate;

		if (Mathf.Abs(rb.velocity.x) < 1f && Mathf.Abs(rb.velocity.y) < 1f)
		{
			animator.SetTrigger("explode");
		}

		if (timer > destroyTime)
		{
			DestroyProjectile();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Damagable dmgable = collision.GetComponent<Damagable>();

		if (dmgable != null)
		{
			Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

			bool gitHit = dmgable.Hit(dmg, deliveredKnockback);

			if (gitHit)
			{
				animator.SetTrigger("explode");
			}
		}
	}

	public void PlaySFX()
	{
		AudioManager.Instance.PlaySFX(AudioManager.Instance.fireBallExplode);
	}

	public void DestroyProjectile()
	{
		Destroy(gameObject);
	}
}
