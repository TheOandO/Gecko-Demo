using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch_B : MonoBehaviour, IBossZone
{
	Damagable dmgable;
	Animator animator;
	Collider2D witchCol;
	[SerializeField] Collider2D col;

	public DetectionZone attackZone { get; set; }
	private GameObject player;

	[SerializeField] private List<WitchSpikes> spikeList;

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
		GameManager.Instance.bwitch = this;

		dmgable = GetComponent<Damagable>();
		witchCol = GetComponent<Collider2D>();
		animator = GetComponent<Animator>();
		player = GameObject.FindGameObjectWithTag("Player");
	}

	private void Update()
	{
		if (dmgable.IsAlive)
		{
			HasTarget = attackZone.detectedColl.Count > 0;
			col.enabled = true;
			witchCol.enabled = true;

			UpdateDirectionTowardsPlayer();
			if (HasTarget)
			{
				foreach (var spike in spikeList)
				{
					spike.isActive = true;
				}
			}
			else
			{
				foreach (var spike in spikeList)
				{
					spike.isActive = false;

					spike.ResetCollider();
				}
			}
		}
		else
		{
			col.enabled = false;
			witchCol.enabled = false;
		}
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

	#region Save/Load

	public void Save(ref BWitchSaveData data)
	{
		data.Health = dmgable.Health;
	}

	public void Load(BWitchSaveData data)
	{
		dmgable.Health = data.Health;
	}

	#endregion
}

[System.Serializable]
public struct BWitchSaveData
{
	public int Health;
}
