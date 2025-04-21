using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnlock : MonoBehaviour
{
	public bool dashUnlocked { get; private set; }
	public bool rangedUnlocked { get; private set; }

	public PlayerController player;

	private void Start()
	{
		GameManager.Instance.playerUnlock = this;

		player = GetComponentInParent<PlayerController>();

		if (GameManager.Instance.sceneData != null && GameManager.Instance.sceneData.Data != null)
		{
			var sceneData = GameManager.Instance.sceneData.Data;
			if (sceneData.unlockDash)
				UnlockDash();
			else
				dashUnlocked = false;

			if (sceneData.unlockRanged)
				UnlockRanged();
			else
				rangedUnlocked = false;
		}
		else
		{
			Debug.Log("SceneData is not assigned or is missing!");
		}

		if (!dashUnlocked)
			player.dashEnabled = false;

		if (!rangedUnlocked)
			player.rangedEnabled = false;
	}

	public void UnlockDash()
	{
		dashUnlocked = true;
		player.dashEnabled = true;
	}

	public void UnlockRanged()
	{
		rangedUnlocked = true;
		player.rangedEnabled = true;
	}


	#region Save/Load

	public void Save(ref PlayerUnlockData data)
	{
		data.dashUnlocked = dashUnlocked;
		data.rangedUnlocked = rangedUnlocked;
	}

	public void Load(PlayerUnlockData data)
	{
		dashUnlocked = data.dashUnlocked;
		rangedUnlocked = data.rangedUnlocked;

		if (dashUnlocked)
		{
			UnlockDash();
		}

		if (rangedUnlocked)
		{
			UnlockRanged();
		}
	}

	#endregion
}

[System.Serializable]
public struct PlayerUnlockData
{
	public bool dashUnlocked;
	public bool rangedUnlocked;
}
