using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	[SerializeField] private SceneDataSO[] sceneDataSOArray;
	private Dictionary<string, int> sceneIDToIndex = new Dictionary<string, int>();

	private void Awake()
	{
		GameManager.Instance.sceneLoader = this;

		PopulateSceneMap();
	}

	private void PopulateSceneMap()
	{
		foreach (var sceneDataSO in sceneDataSOArray)
		{
			sceneIDToIndex[sceneDataSO.sceneName] = sceneDataSO.sceneIndex;
		}
	}

	public void LoadSceneByIndex(string savedSceneId)
	{
		if (sceneIDToIndex.TryGetValue(savedSceneId, out int  sceneIndex))
		{
			SceneManager.LoadScene(sceneIndex);
		}
		else
		{
			Debug.Log($"No Scene with id: {savedSceneId}");
		}
	}

	public async Task LoadSceneByIndexAsync(string savedSceneId)
	{
		if (sceneIDToIndex.TryGetValue(savedSceneId, out int sceneIndex))
		{
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

			while (!asyncLoad.isDone) 
			{
				if (asyncLoad.progress >= 0.9f)
				{
					asyncLoad.allowSceneActivation = true;
					break;
				}
				await Task.Yield();
			}
		}
		else
		{
			Debug.Log($"No Scene with id: {savedSceneId}");
		}
	}
}
