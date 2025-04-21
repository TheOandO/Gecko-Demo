using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchR_Test : MonoBehaviour, IBossZone
{
	Animator animator;
	ProjectileFire projectile;
	Damagable dmgable;

	public DetectionZone attackZone { get; set; }

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

			if (HasTarget)
			{
				UpdateDirectionTowardsPlayer();
				animator.SetTrigger("attack");
			}

		}

	}
	public void Shoot()
	{
		projectile.FireProjectile();
	}

	public void ResetRWitch()
	{
		animator.ResetTrigger("attack");
		_hasTarget = false;
		animator.SetBool(AnimationStrings.hasTarget, false);
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
}
