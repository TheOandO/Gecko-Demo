using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockTrigger : MonoBehaviour
{
	[SerializeField] private bool unlockDash = false;
	[SerializeField] private bool unlockRanged = false;

	public void UnlockAbilities()
	{
		if (unlockDash)
		{
			GameManager.Instance.playerUnlock.UnlockDash();
		}

		if (unlockRanged)
		{
			GameManager.Instance.playerUnlock.UnlockRanged();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			UnlockAbilities();
		}
	}
}
