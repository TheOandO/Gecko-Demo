using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchW_Test : MonoBehaviour, IBossZone
{
	Damagable dmgable;
	Animator animator;
	WitchLaser witchLaser;
	LineRenderer lineRenderer;

	private GameObject player;

	public DetectionZone attackZone { get; set; }

	[SerializeField] private Transform crosshairPos;

	public bool _hasTarget = false;
	public bool onCooldown = false;

	public float cooldownTime = 2f;

	public float refreshRate = 1f;
	private float refreshTimer = 0f;

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
		animator = GetComponent<Animator>();
		dmgable = GetComponent<Damagable>();
		witchLaser = GetComponentInChildren<WitchLaser>();
		lineRenderer = GetComponent<LineRenderer>();

		player = GameObject.FindGameObjectWithTag("Player");

	}

	void Update()
	{
		if (dmgable.IsAlive)
		{
			HasTarget = attackZone.detectedColl.Count > 0;

			if (HasTarget && !onCooldown)
			{
				refreshTimer += Time.deltaTime;

				if (refreshTimer >= refreshRate)
				{
					refreshTimer = 0f;
					crosshairPos.position = player.transform.position;

					animator.SetTrigger("Attack");
				}
				TurnOnLaserPreWarm();
			}

			else if (HasTarget && onCooldown)
			{
				TurnOffLaserPreWarm();
			}

			//if (HasTarget)
			//{
			//	TurnOnLaserPreWarm();
			//}
		}

	}

	public void Shoot()
	{
		witchLaser.ShootLaser(crosshairPos);
		StartCoroutine(LaserCooldown());
	}

	private IEnumerator LaserCooldown()
	{
		onCooldown = true;
		animator.SetBool("onCooldown", onCooldown);
		yield return new WaitForSeconds(cooldownTime);
		onCooldown = false;
		animator.SetBool("onCooldown", onCooldown);
	}

	public void TurnOnLaserPreWarm()
	{
		if (!lineRenderer.enabled)
		{
			lineRenderer.enabled = true;
		}

		lineRenderer.SetPosition(0, crosshairPos.position);
		lineRenderer.SetPosition(1, transform.position);
	}

	public void TurnOffLaserPreWarm()
	{
		if (lineRenderer.enabled)
		{
			lineRenderer.enabled = false;
		}
	}

	public void ResetWWitch()
	{
		TurnOffLaserPreWarm();
		onCooldown = false;

		animator.ResetTrigger("Attack");
		animator.SetBool("onCooldown", false);

		_hasTarget = false;
		animator.SetBool(AnimationStrings.hasTarget, false);

		crosshairPos.position = transform.position;

		refreshTimer = 0f;
	}
}
