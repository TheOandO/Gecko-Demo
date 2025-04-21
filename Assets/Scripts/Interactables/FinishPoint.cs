using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FinishPoint : MonoBehaviour
{
	[SerializeField] private GameTimer timer; // Reference to the Timer script
	[SerializeField] private WinUI winUI;
	[SerializeField] private GameObject winPanel;
	[SerializeField] private PlayerInput playerInput;
	[SerializeField] private GameObject defaultButton;

	[SerializeField] private int nextLevelUnlock = 2;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			ActivatePanel();
		}
	}

	public void ActivatePanel()
	{
		UnlockNextLevel();
		ShowWinPanel();
	}

	private void ShowWinPanel()
	{
		EventSystem.current.SetSelectedGameObject(defaultButton);
		playerInput.DeactivateInput();
		winPanel.SetActive(true);
		timer.PauseTimer();
		float timeSpent = timer.GetElapsedTime();

		winUI.ShowWinPanel(timeSpent);

		winUI.SetMainMenuButtonAction(() => GoToMainMenu());
		winUI.SetNextStageButtonAction(() => NextStage());
	}

	public void GoToMainMenu()
	{
		GameManager.Instance.sceneLoader.LoadSceneByIndex("menu");
	}

	private void UnlockNextLevel()
	{
		int currentUnlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);

		if (nextLevelUnlock > currentUnlockedLevels)
		{
			PlayerPrefs.SetInt("UnlockedLevels", nextLevelUnlock);
			PlayerPrefs.Save();
		}
	}

	public void NextStage()
	{
		SceneController.instance.NextStage();
	}
}
