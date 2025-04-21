using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
	[SerializeField] private GameObject winPanel;
	[SerializeField] private TextMeshProUGUI timeSpentText;
	[SerializeField] private TextMeshProUGUI deathCountText;
	[SerializeField] private Button mainMenuButton;
	[SerializeField] private Button nextStageButton;

	Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();

		winPanel.SetActive(false);
	}

	public void ShowWinPanel(float elapsedTime)
	{
		winPanel.SetActive(true);

		if (winPanel.activeInHierarchy)
		{
			animator.Play("winAnim_Start");

			// Format and display the time taken from the timer
			int minutes = Mathf.FloorToInt(elapsedTime / 60);
			int seconds = Mathf.FloorToInt(elapsedTime % 60);
			int milliseconds = Mathf.FloorToInt((elapsedTime * 1000) % 1000);

			timeSpentText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);

			string saveFile = SaveSystem.SaveFileName();
			if (File.Exists(saveFile))
			{
				SaveSystem.SaveData saveData = SaveSystem.LoadLevelData(saveFile);

				deathCountText.text = $"x{saveData.deathsData.deathCount}";
			}
			else
			{
				deathCountText.text = "x00"; // Default
			}
		}

	}

	public void SetMainMenuButtonAction(System.Action onMainMenu)
	{
		mainMenuButton.onClick.RemoveAllListeners();
		mainMenuButton.onClick.AddListener(() => onMainMenu());
	}

	public void SetNextStageButtonAction(System.Action onNextStage)
	{
		nextStageButton.onClick.RemoveAllListeners();
		nextStageButton.onClick.AddListener(() => onNextStage());
	}

	public void HideWinPanel()
	{
		winPanel.SetActive(false);
	}
}
