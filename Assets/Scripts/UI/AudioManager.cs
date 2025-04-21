using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;

	[SerializeField] AudioSource musicSource;
	[SerializeField] AudioSource ambienceSource;
	[SerializeField] AudioSource sfxSource;

	public AudioClip death;
	public AudioClip checkpoint;
	public AudioClip jump;
	public AudioClip dash;
	public AudioClip grab;
	public AudioClip grapple;
	public AudioClip voice;
	public AudioClip fireBallExplode;
	public AudioClip spitBallExplode;
	public AudioClip pause;
	public AudioClip unpause;
	public AudioClip menuNavigate;
	public AudioClip menuSelect;
	public AudioClip menuConfirm;
	public AudioClip submarine;
	public AudioClip cat;
	public AudioClip rumble;
	public AudioClip tvStatic;

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

	public void PlaySFX(AudioClip clip)
	{
		sfxSource.PlayOneShot(clip);
	}
}
