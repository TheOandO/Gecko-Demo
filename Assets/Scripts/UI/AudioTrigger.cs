using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
	public AudioClip newMusic;
	public AudioPlayer audioPlayer;

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			if (audioPlayer != null)
			{
				audioPlayer.ChangeMusic(newMusic);
				gameObject.SetActive(false);
			}
		}
	}
}
