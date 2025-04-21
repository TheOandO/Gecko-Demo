using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch_R : MonoBehaviour, IBossZone
{
	Animator animator;
	ProjectileFire projectile;
	Damagable dmgable;
	Collider2D witchCol;
	[SerializeField]Collider2D col;
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
		GameManager.Instance.rwitch = this;

		projectile = GetComponentInChildren<ProjectileFire>();
		animator = GetComponent<Animator>();
		dmgable = GetComponent<Damagable>();
		witchCol = GetComponent<Collider2D>();
		player = GameObject.FindGameObjectWithTag("Player");
	}

	private void Update()
	{
		if (dmgable.IsAlive)
		{
			HasTarget = attackZone.detectedColl.Count > 0;
			col.enabled = true;
			witchCol.enabled = true;

			if (HasTarget)
			{
				UpdateDirectionTowardsPlayer();
				animator.SetTrigger("attack");
			}
		}
		else
		{
			witchCol.enabled = false;
			col.enabled = false;
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

	public void ResetRWitch()
	{
		animator.ResetTrigger("attack");
		_hasTarget = false;
		animator.SetBool(AnimationStrings.hasTarget, false);
	}

	#region Save/Load

	public void Save(ref RWitchSaveData data)
	{
		data.Health = dmgable.Health;
	}

	public void Load(RWitchSaveData data)
	{
		dmgable.Health = data.Health;
	}

	#endregion
}

[System.Serializable]
public struct RWitchSaveData
{
	public int Health;
}
