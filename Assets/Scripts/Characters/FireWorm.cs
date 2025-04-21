using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWorm : MonoBehaviour
{
	Animator animator;
	ProjectileFire projectile;
	Damagable dmgable;

	public DetectionZone attackZone;

	private GameObject player;

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

	private void Start()
	{
		projectile = GetComponentInChildren<ProjectileFire>();
		animator = GetComponent<Animator>();
		dmgable = GetComponent<Damagable>();

		player = GameObject.FindGameObjectWithTag("Player");
	}

	private void Update()
	{
		if (dmgable.IsAlive)
		{
			HasTarget = attackZone.detectedColl.Count > 0;
			UpdateDirectionTowardsPlayer();

			if (HasTarget)
			{
				animator.SetTrigger("attack");
			}
		}

	}
	public void Shoot()
	{
		projectile.FireProjectile();
	}

	private void UpdateDirectionTowardsPlayer()
	{
		Vector3 locScale = transform.localScale;

		if (player != null)
		{
			if (player.transform.position.x > transform.position.x)
			{
				transform.localScale = new Vector3(Mathf.Abs(locScale.x), locScale.y, locScale.z);
			}
			else if (player.transform.position.x < transform.position.x)
			{
				transform.localScale = new Vector3(-Mathf.Abs(locScale.x), locScale.y, locScale.z);
			}
		}
	}

	public void ResetFireWorm()
	{
		animator.Play("worm_idle");

		HasTarget = false;
	}
}
