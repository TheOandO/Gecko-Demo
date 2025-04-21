using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TimelineTrigger : MonoBehaviour
{
	public GameObject timeline;
	public CinemachineVirtualCamera timelineCam;
	private CinemachineVirtualCamera currentCam;

	//public List<GameObject> characters = new List<GameObject>();
	//public List<Transform> targetPositions = new List<Transform>();

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (timelineCam == null)
		{
			timeline.SetActive(true);
		} else
		{
			currentCam = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;

			timelineCam.enabled = true;
			timeline.SetActive(true);
			currentCam.enabled = false;
		}

		//if (characters.Count != targetPositions.Count)
		//{
		//	Debug.LogError("The number of characters and target positions don't match!");
		//	return;
		//}

		//for (int i = 0; i < characters.Count; i++)
		//{
		//	if (characters[i] != null && targetPositions[i] != null)
		//	{
		//		Debug.Log("Activated");
		//		characters[i].transform.position = targetPositions[i].position;
		//	}
		//}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		gameObject.SetActive(false);
	}
}
