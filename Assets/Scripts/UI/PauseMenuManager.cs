using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
	[SerializeField] GameObject pauseMenu;
	[SerializeField] GameObject pausePanel;
	[SerializeField] private GameObject settingsMenuCanvas;

	[Header("Default Option Menu")]
	[SerializeField] private GameObject mainMenuFirst;
	[SerializeField] private GameObject optionMenuFirst;

	[Header("Disable Player Move")]
	[SerializeField] PlayerInput inputAction;

	GameMiscSettings gameMiscSettings;

	private GameObject lastSelectedButton;

	private bool isPaused = false;

	private void Start()
	{
		lastSelectedButton = EventSystem.current.currentSelectedGameObject;

		gameMiscSettings = GetComponent<GameMiscSettings>();
		gameMiscSettings.LoadGameSpeed();
	}
	private void Update()
	{
		if (InputManager.Instance.OpenMenuInput)
		{
			if (!isPaused)
			{
				Pause();
			}

		}

		else if (InputManager.Instance.CloseMenuInput)
		{
			if (isPaused)
			{
				Resume();
			}
		}

		GameObject currentSelectedButton = EventSystem.current.currentSelectedGameObject;

		if (currentSelectedButton != null && currentSelectedButton != lastSelectedButton)
		{
			AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelect);

			// Update the last selected button
			lastSelectedButton = currentSelectedButton;
		}
	}
	public void TogglePause()
	{
		if (!isPaused)
		{
			Pause();
		}
		else
		{
			Resume();
		}
	}

	public void Pause()
	{
		Time.timeScale = 0;
		isPaused = true;

		AudioManager.Instance.PlaySFX(AudioManager.Instance.pause);

		inputAction.SwitchCurrentActionMap("UI");

		OpenMenu();
	}

	public void Resume()
	{
		float time = gameMiscSettings.GetStoredTimeScale();

		if (time != 1 && time <= 2 && time >= 0.2)
			Time.timeScale = time;
		else
			Time.timeScale = 1;

		isPaused = false;

		AudioManager.Instance.PlaySFX(AudioManager.Instance.unpause);

		inputAction.SwitchCurrentActionMap("Player");

		CloseAllMenus();
	}

	public void Home()
	{
		SceneManager.LoadScene(0);
		Time.timeScale = 1;

		GameManager.Instance.deathCounter.UpdateDeathCount();
		GameManager.Instance.timer.UpdateElapsedTime();
	}

	public void Restart()
	{
		GameManager.Instance.timer.UpdateElapsedTime();
		GameManager.Instance.deathCounter.UpdateDeathCount();

		Time.timeScale = 1;
		GameManager.Instance.LoadAsync();
		TogglePause();
	}

	public void OpenMenu()
	{
		pauseMenu.SetActive(true);
		pausePanel.SetActive(true);
		settingsMenuCanvas.SetActive(false);

		SetEventSystemGOMain();

	}

	public void CloseAllMenus()
	{
		pauseMenu.SetActive(false);
		settingsMenuCanvas.SetActive(false);
		pausePanel.SetActive(true);

		EventSystem.current.SetSelectedGameObject(null);
	}

	public void SetEventSystemGOMain()
	{
		EventSystem.current.SetSelectedGameObject(mainMenuFirst);
	}

	public void SetEventSystemGOOption()
	{
		EventSystem.current.SetSelectedGameObject(optionMenuFirst);
	}
}
