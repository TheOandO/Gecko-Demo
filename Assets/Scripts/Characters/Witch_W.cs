using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch_W : MonoBehaviour, IBossZone
{
	Damagable dmgable;
	Animator animator;
	WitchLaser witchLaser;
	LineRenderer lineRenderer;
	Collider2D witchCol;
	[SerializeField] Collider2D col;

	private GameObject player;

	private Coroutine laserCoroutine;

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
		GameManager.Instance.wwitch = this;

		animator = GetComponent<Animator>();
		dmgable = GetComponent<Damagable>();
		witchCol = GetComponent<Collider2D>();
		witchLaser = GetComponentInChildren<WitchLaser>();
		lineRenderer = GetComponent<LineRenderer>();

		player = GameObject.FindGameObjectWithTag("Player");

	}

	void Update()
    {
		if (dmgable.IsAlive)
		{
			HasTarget = attackZone.detectedColl.Count > 0;
			col.enabled = true;
			witchCol.enabled = true;

			if (HasTarget && !onCooldown)
			{
				if (!onCooldown)
				{
					refreshTimer += Time.deltaTime;

					if (refreshTimer >= refreshRate)
					{
						refreshTimer = 0f;
						crosshairPos.position = player.transform.position;

						animator.SetTrigger("Attack");
					}
					//TurnOnLaserPreWarm();
				}

				else if (onCooldown)
                {
					TurnOffLaserPreWarm();
                }

				UpdateDirectionTowardsPlayer();
			}

			//if (HasTarget)
			//{
			//	TurnOnLaserPreWarm();
			//}
		}
		else
		{
			col.enabled = false;
			witchCol.enabled = false;
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

		if (laserCoroutine != null)
		{
			StopCoroutine(laserCoroutine);
		}
		laserCoroutine = StartCoroutine(LaserCountdown());
	}

	private IEnumerator LaserCountdown()
	{
		yield return new WaitForSeconds(2);

		if (lineRenderer.enabled)
		{
			TurnOffLaserPreWarm();
		}

	}

	public void TurnOffLaserPreWarm()
	{
		if (lineRenderer.enabled)
		{
			lineRenderer.enabled = false;

			if (laserCoroutine != null)
			{
				StopCoroutine(laserCoroutine);
				laserCoroutine = null;
			}
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

	public void ResetWWitch()
	{
		TurnOffLaserPreWarm();
		onCooldown = false;

		animator.ResetTrigger("Attack");
		animator.SetBool("onCooldown", false);

		_hasTarget = false;
		animator.SetBool(AnimationStrings.hasTarget, false);

		crosshairPos.position = transform.position;
		witchLaser.DestroyLaser();

		refreshTimer = 0f;
	}

	#region Save/Load

	public void Save(ref WWitchSaveData data)
	{
		data.Health = dmgable.Health;
	}

	public void Load(WWitchSaveData data)
	{
		dmgable.Health = data.Health;
	}

	#endregion
}

[System.Serializable]
public struct WWitchSaveData
{
	public int Health;
}
