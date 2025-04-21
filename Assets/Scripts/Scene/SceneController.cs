using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
	public static SceneController instance;
	[SerializeField] Animator animator;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void NextStage()
	{
		StartCoroutine(LoadNextScene());
	}

	IEnumerator LoadNextScene()
	{
		animator.SetTrigger(AnimationStrings.endTrigger);
		yield return new WaitForSeconds(1);
		animator.SetTrigger(AnimationStrings.startTrigger);
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void NextStageNoAnim()
	{
		StartCoroutine(LoadNextSceneNoANim());
	}

	IEnumerator LoadNextSceneNoANim()
	{
		yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
