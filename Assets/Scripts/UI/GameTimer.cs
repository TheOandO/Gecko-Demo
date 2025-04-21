using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI timerText;
	float elapsedTime;

	public bool isPaused = false;

	private void Start()
	{
		GameManager.Instance.timer = this;
	}

	private void Update()
	{
		if (!isPaused)
		{
			elapsedTime += Time.deltaTime;
			UpdateTimerDisplay();
		}
	}

	public void PauseTimer()
	{
		isPaused = true;
	}

	public void ResumeTimer()
	{
		isPaused = false;
	}

	public void ResetTimer()
	{
		elapsedTime = 0f;
		UpdateTimerDisplay();
	}

	private void UpdateTimerDisplay()
	{
		int minutes = Mathf.FloorToInt(elapsedTime / 60);
		int seconds = Mathf.FloorToInt(elapsedTime % 60);
		int milliseconds = Mathf.FloorToInt((elapsedTime * 1000) % 1000);

		timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
	}

	public void UpdateElapsedTime()
	{
		float currentTime = GameManager.Instance.timer.GetElapsedTime();

		// Overwrite the saved time with the current time
		TimerSaveData updatedTimeData = new TimerSaveData();
		updatedTimeData.elapsedTime = currentTime;

		// Save the updated data into the save system
		Save(ref updatedTimeData);

		// Now overwrite the elapsedTimeData in the SaveSystem (without saving all other data)
		SaveSystem.SaveData data = SaveSystem.LoadLevelData(SaveSystem.SaveFileName());
		data.elapsedTimeData = updatedTimeData;

		// Save the updated data
		File.WriteAllText(SaveSystem.SaveFileName(), JsonUtility.ToJson(data, true));
	}

	public float GetElapsedTime()
	{
		return elapsedTime;
	}

	#region Save/Load

	public void Save(ref TimerSaveData data)
	{
		data.elapsedTime = GetElapsedTime();
	}

	public void Load(TimerSaveData data)
	{
		elapsedTime = data.elapsedTime;
	}

	#endregion
}

[System.Serializable]
public struct TimerSaveData
{
	public float elapsedTime;
}
