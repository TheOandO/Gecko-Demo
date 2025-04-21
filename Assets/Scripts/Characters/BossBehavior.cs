using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
	[SerializeField] private List<Transform> teleportPositions;
	[SerializeField] private List<DetectionZone> detectionZones;
	[SerializeField] private List<int> healthThresholds;

	private int posIndex = 0;
	private int zoneIndex = 0;
	private int lastThresholdIndex = -1;
	private int damageTaken = 0;

	public bool canTeleportBack = true;

	Damagable damagable;
	Animator animator;

	IBossZone attackZoneHolder;

	private void Awake()
	{
		attackZoneHolder = GetComponent<IBossZone>();
		animator = GetComponent<Animator>();
		damagable = GetComponent<Damagable>();
		if (detectionZones.Count > 0)
		{
			attackZoneHolder.attackZone = detectionZones[0];
		}

		if (teleportPositions.Count > 0)
		{
			transform.position = teleportPositions[0].position;
		}
	}

	private void Update()
	{
		CheckHealth();
	}

	private void CheckHealth()
	{
		for (int i = 0; i < healthThresholds.Count; i++)
		{
			if (damagable.Health <= healthThresholds[i] && i > lastThresholdIndex)
			{
				lastThresholdIndex = i; // Update the last triggered threshold
				TeleportToIndex(i);
				ChangeDetectionZone(i);
				break;
			}
		}

		// Check if health rises above the last threshold (for teleporting back)
		for (int i = lastThresholdIndex; i >= 0; i--)
		{
			if (damagable.Health > healthThresholds[i])
			{
				lastThresholdIndex = i - 1; // Update the last threshold index
				TeleportToIndex(lastThresholdIndex);
				ChangeDetectionZone(lastThresholdIndex);

				Debug.Log("Current index: " + i);
				break;
			}
		}
	}

	private void TeleportToIndex(int index)
	{
		if (teleportPositions.Count == 0) return;

		// Teleport the boss to the specific position based on the index
		posIndex = index;
		transform.position = teleportPositions[posIndex].position;
	}

	private void ChangeDetectionZone(int index)
	{
		if (detectionZones.Count == 0) return;

		// Change the detection zone based on the index
		zoneIndex = index;
		attackZoneHolder.attackZone = detectionZones[zoneIndex];
	}

	public void OnBossDamaged(int damage, Vector2 knockback)
	{
		animator.SetTrigger(AnimationStrings.hitTrigger);
		canTeleportBack = true;
		damageTaken += damage;

		Debug.Log("dmg taken: " + damageTaken);
	}

	public void ResetBoss()
	{
		if (canTeleportBack)
		{
			damagable.Health += damageTaken;
			CheckHealth();

			damagable.IsAlive = true;
			damageTaken = 0;
		}

		//animator.Play("Idle");
	}

	public void DisableTeleportBack()
	{
		canTeleportBack = false;
		damageTaken = 0;
	}
}
