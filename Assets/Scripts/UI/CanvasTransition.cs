using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTransition : MonoBehaviour
{
	[SerializeField] private Animator startCanvasAnimator;
	[SerializeField] private Animator levelSelectionCanvasAnimator;
	[SerializeField] private GameObject startCanvas;
	[SerializeField] private GameObject levelSelectionCanvas;

	public void ShowLevelSelection()
	{
		StartCoroutine(SwitchCanvas(startCanvasAnimator, levelSelectionCanvasAnimator, startCanvas, levelSelectionCanvas));
	}

	public void ShowStartMenu()
	{
		StartCoroutine(SwitchCanvas(levelSelectionCanvasAnimator, startCanvasAnimator, levelSelectionCanvas, startCanvas));
	}

	private IEnumerator SwitchCanvas(Animator currentCanvasAnimator, Animator nextCanvasAnimator, GameObject currentCanvas, GameObject nextCanvas)
	{
		currentCanvasAnimator.SetTrigger("End");

		yield return new WaitForSeconds(currentCanvasAnimator.GetCurrentAnimatorStateInfo(0).length);

		currentCanvas.SetActive(false);

		nextCanvas.SetActive(true);
		nextCanvasAnimator.SetTrigger("Start");
	}
}
