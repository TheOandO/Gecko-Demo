using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorDisabler : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collider)
	{
		BossBehavior[] bossBehavior = FindObjectsOfType<BossBehavior>();

		if (collider.CompareTag("Player"))
		{
			foreach (var boss in bossBehavior)
			{
				boss.DisableTeleportBack();
			}
		}

		this.enabled = false;
	}
}
