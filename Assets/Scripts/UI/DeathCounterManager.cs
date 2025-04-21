using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class DeathCounterManager : MonoBehaviour
{
	public static DeathCounterManager Instance { get; private set; }
	private int deathCount = 0;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}


	private void Start()
	{
		GameManager.Instance.deathCounter = this;
	}

	public void IncrementDeathCount()
	{
		deathCount++;
	}

	public int GetDeathCount()
	{
		return deathCount;
	}

	public void UpdateDeathCount()
	{
		int currentDeathCount = GetDeathCount();

		// Overwrite the saved death count with the current count
		DeathSaveData updatedDeathData = new DeathSaveData();
		updatedDeathData.deathCount = currentDeathCount;

		Save(ref updatedDeathData);  // Save the updated death count

		// Overwrite the deathCount data in the SaveSystem
		SaveSystem.SaveData data = SaveSystem.LoadLevelData(SaveSystem.SaveFileName());
		data.deathsData = updatedDeathData;

		File.WriteAllText(SaveSystem.SaveFileName(), JsonUtility.ToJson(data, true));
	}

	#region Save/Load

	public void Save(ref DeathSaveData data)
	{
		data.deathCount = GetDeathCount();
	}

	public void Load(DeathSaveData data)
	{
		deathCount = data.deathCount;
	}

	#endregion
}

[System.Serializable]
public struct DeathSaveData
{
	public int deathCount;
}
