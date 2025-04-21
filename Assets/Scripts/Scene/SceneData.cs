using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneData : MonoBehaviour
{
	public SceneDataSO Data;

	private void Awake()
	{
		GameManager.Instance.sceneData = this;
	}

	#region Save/Load

	public void Save(ref SceneSaveData data)
	{
		data.SceneID = Data.sceneName;
	}

	public void Load(SceneSaveData data)
	{
		GameManager.Instance.sceneLoader.LoadSceneByIndex(data.SceneID);
	}

	public async Task LoadAsync(SceneSaveData data)
	{
		await GameManager.Instance.sceneLoader.LoadSceneByIndexAsync(data.SceneID);
	}

	public Task Wait4Scene()
	{
		TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

		UnityEngine.Events.UnityAction<Scene, LoadSceneMode> loaderHandler = null;

		loaderHandler = (scene, mode) =>
		{
			taskCompletion.SetResult(true);
			SceneManager.sceneLoaded -= loaderHandler;
		};

		SceneManager.sceneLoaded += loaderHandler;

		return taskCompletion.Task;
	}

	#endregion
}

[System.Serializable]
public struct SceneSaveData
{
	public string SceneID;
}
