using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public Button[] levelButtons;
	[SerializeField] private TextMeshProUGUI timeInfoText;
	[SerializeField] private TextMeshProUGUI stageNameInfoText;
	[SerializeField] private TextMeshProUGUI deathsInfoText;
	public Button loadButton;
	public Button startNewButton;
	public Button deleteButton;

	private string selectedLevel;

	[SerializeField] private GameObject defaultButton;

	private void Awake()
	{
		LockButtons();

		EventSystem.current.SetSelectedGameObject(defaultButton);
	}

	private void LockButtons()
	{
		int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevels", 1);

		for (int i = 0; i < levelButtons.Length; i++)
		{
			levelButtons[i].interactable = false;
		}

		// Enable unlocked levels based on PlayerPrefs
		for (int i = 0; i < unlockedLevel; i++)
		{
			int levelIndex = i;
			levelButtons[i].interactable = true;
		}
		
		startNewButton.interactable = false;
		loadButton.interactable = false;
	}



	public void OnLevelSelected(SceneDataSO levelData)
	{
		selectedLevel = levelData.sceneName;
		UpdateLevelInfo();
	}

	private void UpdateLevelInfo()
	{
		startNewButton.interactable = true;

		string saveFile = GetSaveFileNameForLevel(selectedLevel);

		stageNameInfoText.text = GetStageName(selectedLevel);

		if (File.Exists(saveFile))
		{
			SaveSystem.SaveData saveData = SaveSystem.LoadLevelData(saveFile);
			DisplaySaveInfo(saveData);
			loadButton.interactable = true;
		}
		else
		{
			timeInfoText.text = "No saved data.";
			loadButton.interactable = false;
		}
	}

	private string GetStageName(string levelName)
	{
		switch (levelName)
		{
			case "scene 1":
				return "Stage 1: The Maze of Madness";
			case "scene 2":
				return "Stage 2: Fungi Business";
			case "scene 3":
				return "Stage 3: The Confrontation";
			case "prologue":
				return "Prologue: The Neverending Forest";
			default:
				return "Unknown Stage";
		}
	}

	private void DisplaySaveInfo(SaveSystem.SaveData saveData)
	{
		int minutes = Mathf.FloorToInt(saveData.elapsedTimeData.elapsedTime / 60);
		int seconds = Mathf.FloorToInt(saveData.elapsedTimeData.elapsedTime % 60);
		int milliseconds = Mathf.FloorToInt((saveData.elapsedTimeData.elapsedTime * 1000) % 1000);

		timeInfoText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);

		deathsInfoText.text = $"x{saveData.deathsData.deathCount}";
	}

	public async void OnLoadLevelPressed()
	{
		if (selectedLevel != null)
		{
			string saveFile = GetSaveFileNameForLevel(selectedLevel); 
			Debug.Log(saveFile);
			if (File.Exists(saveFile))
			{
				Debug.Log($"Load gud: {selectedLevel}");
				await GameManager.Instance.LoadSelectedAsync(saveFile);
			}
			else
			{
				Debug.Log($"No save file found for the selected scene: {selectedLevel}");
			}
		}
		else
		{
			Debug.Log("No level selected to load.");
		}
	}

	public async void OnStartNewLevelPressed()
	{
		if (selectedLevel != null)
		{
			string saveFile = GetSaveFileNameForLevel(selectedLevel);

			if (File.Exists(saveFile))
			{
				File.Delete(saveFile);
				Debug.Log($"Save file for {selectedLevel} deleted.");
			}
			else
			{
				Debug.Log($"No save file found for {selectedLevel}.");
			}

			if (selectedLevel == "prologue")
			{
				await GameManager.Instance.sceneLoader.LoadSceneByIndexAsync("cutscene");
			}
			else
			{
				await GameManager.Instance.sceneLoader.LoadSceneByIndexAsync(selectedLevel);
			}
		}
		else
		{
			Debug.Log("No level selected to start.");
		}
	}

	public void OnDeleteAll()
	{
		string saveDirectory = Application.persistentDataPath;
		if (Directory.Exists(saveDirectory))
		{
			string[] saveFiles = Directory.GetFiles(saveDirectory, "save_*.sav");
			foreach (string file in saveFiles)
			{
				File.Delete(file);
				Debug.Log($"Deleted save file: {file}");
			}
		}

		PlayerPrefs.SetInt("UnlockedLevels", 1);
		PlayerPrefs.Save();
		Debug.Log("All save data deleted and PlayerPrefs reset.");

		LockButtons();
	}

	private string GetSaveFileNameForLevel(string levelName)
	{
		return Application.persistentDataPath + "/save_" + levelName + ".sav";
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
