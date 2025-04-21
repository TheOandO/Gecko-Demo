using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource loopMusicSource;
	public List<AudioSource> loopAmbianceSources = new List<AudioSource>();

	private void Awake()
	{
		if (loopAmbianceSources.Count > 0)
		{
			loopMusicSource = null;
		}
		else
		{
			loopMusicSource.Play();
		}
	}

	void Start()
    {
		PlayAll();
    }

	public void PlayAll()
	{
		if (loopAmbianceSources.Count > 0)
		{
			foreach (AudioSource source in loopAmbianceSources)
			{
				if (!source.isPlaying)
				{
					source.Play();
				}
			}
		}

	}

	public void ChangeMusic(AudioClip newClip)
	{
		if (loopMusicSource != null)
		{
			if (loopMusicSource.clip != newClip)
			{
				loopMusicSource.Stop();
				loopMusicSource.clip = newClip;
				loopMusicSource.Play();
			}
		}

	}
}
