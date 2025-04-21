using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Playables;

public class PlayableDirectorManager : MonoBehaviour
{
	public double skipTime = 12.8;

	private static PlayableDirectorManager instance;
	private PlayableDirector directorCurrent;
	private bool isSkipped = false;
	private float holdTime = 0f;
	private const float requiredHoldTime = 4f; // 3 seconds to skip
	[SerializeField]InputSystemUIInputModule input;

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		if (directorCurrent == null) return; // Ensure the director is assigned

		if (input.cancel)
		{
			holdTime += Time.deltaTime;

			if (holdTime >= requiredHoldTime && !isSkipped)
			{
				SkipTimeline();
			}
		}
		else
		{
			holdTime = 0f;
		}
	}

	private void SkipTimeline()
	{
		directorCurrent.time = skipTime; // Skip to the desired timeframe
		isSkipped = true;
	}

	public void GetDirector()
	{
		isSkipped = false;
		directorCurrent = GetComponent<PlayableDirector>();
	}
}
