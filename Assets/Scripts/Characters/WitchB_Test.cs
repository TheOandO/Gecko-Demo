using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchB_Test : MonoBehaviour, IBossZone
{
	Damagable dmgable;
	Animator animator;

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

		dmgable = GetComponent<Damagable>();
		animator = GetComponent<Animator>();
		player = GameObject.FindGameObjectWithTag("Player");
	}

	private void Update()
	{
		if (dmgable.IsAlive)
		{
			HasTarget = attackZone.detectedColl.Count > 0;

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
	}
}
