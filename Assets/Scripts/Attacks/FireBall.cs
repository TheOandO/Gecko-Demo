using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireBall : MonoBehaviour
{
	public int dmg = 10;
	public float force;
	public float destroyTime = 5;
	public Vector2 knockback = new Vector2(0, 0);

	private GameObject player;

	private float timer;

	Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			Vector3 dir = player.transform.position - transform.position;
			rb.velocity = new Vector2(dir.x, dir.y).normalized * force;

			float rotation = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0, 0, rotation + 180);
		}

	}

	private void Update()
	{
		timer += Time.deltaTime;

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
				DestroyProjectile();
			}
		}
	}

	public void DestroyProjectile()
	{
		Destroy(gameObject);
	}
}
