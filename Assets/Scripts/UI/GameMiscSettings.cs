using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class GameMiscSettings : MonoBehaviour
{
	[Header("Game Speed")]
	[SerializeField] private Slider timeScaleSlider;
	[SerializeField] private TextMeshProUGUI sliderText;
	private const string GameSpeedPref = "GameSpeed";
	private float storedTimeScale;

	[Header("CRT Settings")]
	[SerializeField] Material mat;
	[SerializeField] Toggle CRTToggle;
	private const string CRTBoolPref = "CRT";

	[Header("Blur Settings")]
	[SerializeField] Toggle blurToggle;
	private double blurAmount = 0.001102;
	private const string BlurBoolPref = "Blur";


	void Start()
	{
		LoadCRT();
		LoadBlur();
	}

	public void SetGameSpeed(float volume)
	{
		float snappedValue = Mathf.Round(volume * 10) / 10f; // Snap to nearest
		timeScaleSlider.value = snappedValue; // Update slider position
		sliderText.text = $"x{snappedValue:F1}"; // Display 1 decimal place

		storedTimeScale = volume;
		PlayerPrefs.SetFloat(GameSpeedPref, snappedValue);
	}

	public float GetStoredTimeScale()
	{
		return storedTimeScale;
	}

	public void LoadGameSpeed()
	{
		if (PlayerPrefs.HasKey(GameSpeedPref))
		{
			float savedTimeScaleVolume = PlayerPrefs.GetFloat(GameSpeedPref);
			float snappedValue = Mathf.Round(savedTimeScaleVolume * 10) / 10f; // Ensure snapping on load

			Time.timeScale = snappedValue;
			timeScaleSlider.value = snappedValue;
			sliderText.text = $"x{snappedValue:F1}";
		}
	}

	public void ToggleCRT()
	{
		if (CRTToggle.isOn)
		{
			mat.SetFloat("_EnableCRT", 1);
			PlayerPrefs.SetFloat(CRTBoolPref, 1);
			blurToggle.interactable = true;
		}
		else
		{
			mat.SetFloat("_EnableCRT", 0);
			PlayerPrefs.SetFloat(CRTBoolPref, 0);
			blurToggle.interactable = false;
		}

	}

	private void LoadCRT()
	{
		if (PlayerPrefs.HasKey(CRTBoolPref))
		{
			float savedCRTVolume = PlayerPrefs.GetFloat(CRTBoolPref);

			mat.SetFloat("_EnableCRT", savedCRTVolume);

			if (savedCRTVolume == 0)
			{
				CRTToggle.isOn = false;
				blurToggle.interactable = false;
			}
			else if (savedCRTVolume == 1)
			{
				CRTToggle.isOn = true;
				blurToggle.interactable = true;
			}
		}
	}

	public void ToggleBlur()
	{
		if (blurToggle.isOn)
		{
			mat.SetFloat("_Blur_Offset", (float)blurAmount);
			PlayerPrefs.SetFloat(BlurBoolPref, (float)blurAmount);
		}
		else
		{
			mat.SetFloat("_Blur_Offset", 0);
			PlayerPrefs.SetFloat(BlurBoolPref, 0);
		}
	}

	private void LoadBlur()
	{
		if (PlayerPrefs.HasKey(BlurBoolPref))
		{
			float savedBlurVolume = PlayerPrefs.GetFloat(BlurBoolPref);

			mat.SetFloat("_Blur_Offset", savedBlurVolume);

			if (savedBlurVolume == 0)
			{
				blurToggle.isOn = false;
			}
			else if (savedBlurVolume == (float)blurAmount)
			{
				blurToggle.isOn = true;
			}
		}
	}
}
