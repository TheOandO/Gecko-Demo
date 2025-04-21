using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
	[SerializeField] private AudioMixer audioMixer;
	[SerializeField] private Slider masterSlider;
	[SerializeField] private Slider musicSlider;
	[SerializeField] private Slider ambianceSlider;
	[SerializeField] private Slider sfxSlider;

	private const string MusicPref = "MusicVolume";
	private const string AmbiancePref = "AmbianceVolume";
	private const string SFXPref = "SFXVolume";
	private const string MasterPref = "MasterVolume";

	private void Start()
	{
		LoadVolumeSettings();
	}

	public void SetMusicVolume(float volume)
	{
		audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20f);
		PlayerPrefs.SetFloat(MusicPref, volume);
	}

	public void SetAmbianceVolume(float volume)
	{
		audioMixer.SetFloat("Ambiance", Mathf.Log10(volume) * 20f);
		PlayerPrefs.SetFloat(AmbiancePref, volume);
	}

	public void SetSFXVolume(float volume)
	{
		audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20f);
		PlayerPrefs.SetFloat(SFXPref, volume);
	}

	public void SetMasterVolume(float volume)
	{
		audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20f);
		PlayerPrefs.SetFloat(MasterPref, volume);
	}

	private void LoadVolumeSettings()
	{
		if (PlayerPrefs.HasKey(MusicPref))
		{
			float savedMusicVolume = PlayerPrefs.GetFloat(MusicPref);
			audioMixer.SetFloat("Music", Mathf.Log10(savedMusicVolume) * 20f);
			musicSlider.value = savedMusicVolume; // Set slider value
		}

		if (PlayerPrefs.HasKey(AmbiancePref))
		{
			float savedAmbianceVolume = PlayerPrefs.GetFloat(AmbiancePref);
			audioMixer.SetFloat("Ambiance", Mathf.Log10(savedAmbianceVolume) * 20f);
			ambianceSlider.value = savedAmbianceVolume; // Set slider value
		}

		if (PlayerPrefs.HasKey(SFXPref))
		{
			float savedSFXVolume = PlayerPrefs.GetFloat(SFXPref);
			audioMixer.SetFloat("SFX", Mathf.Log10(savedSFXVolume) * 20f);
			sfxSlider.value = savedSFXVolume; // Set slider value
		}

		if (PlayerPrefs.HasKey(MasterPref))
		{
			float savedMasterVolume = PlayerPrefs.GetFloat(MasterPref);
			audioMixer.SetFloat("Master", Mathf.Log10(savedMasterVolume) * 20f);
			masterSlider.value = savedMasterVolume; // Set slider value
		}
	}
}
