using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;
	public static GameManager Instance
	{
		get
		{
#if UNITY_EDITOR

			if (!Application.isPlaying)
			{
				return null;
			}

			if (instance == null)
			{
				Instantiate(Resources.Load<GameManager>("GameManager"));
			}

#endif
			return instance;
		}
	}

	public PlayerController playerController { get; set; }
	public PlayerUnlock playerUnlock { get; set; }
	public CameraManager currentCam { get; set; }
	public DeathCounterManager deathCounter { get; set; }
	public GameTimer timer { get; set; }
	public Witch_W wwitch { get; set; }
	public Witch_B bwitch { get; set; }
	public Witch_R rwitch { get; set; }
	public SceneData sceneData { get; set; }
	public SceneLoader sceneLoader { get; set; }

	private bool isSaving;
	private bool isLoading;

	[SerializeField] private GameObject saveIcon;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		Application.targetFrameRate = 60;
	}

	public async void SaveAsync()
	{
		isSaving = true;
		saveIcon.SetActive(true);
		saveIcon.GetComponent<Animator>().Play("upButt_idle");

		await SaveSystem.SaveAsync();

		StartCoroutine(SaveAnimation(saveIcon.GetComponent<Animator>(), "upButt_idle"));
		isSaving = false;
	}

	private IEnumerator SaveAnimation(Animator animator, string animationName)
	{
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		while (stateInfo.IsName(animationName) && stateInfo.normalizedTime < 1.0f)
		{
			yield return null;
			stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		}

		saveIcon.SetActive(false);
	}

	public async void LoadAsync()
	{
		string saveFile = SaveSystem.SaveFileName();  // Get the save file for the current scene

		if (File.Exists(saveFile))  // Check if the save file exists for the current scene
		{
			isLoading = true;
			await SaveSystem.LoadAsync();  // Load the save file for the current scene
			isLoading = false;
		}
		else
		{
			Debug.Log("No save file found for the current scene: " + Instance.sceneData.Data.sceneName);
		}
	}

	public async Task LoadSelectedAsync(string saveFile)
	{
		if (File.Exists(saveFile))
		{
			isLoading = true;
			await SaveSystem.LoadSelectedAsync(saveFile);  // Load the save file for the selected level
			isLoading = false;
		}
		else
		{
			Debug.Log($"No save file found for the selected scene: {saveFile}");
		}
	}
}
