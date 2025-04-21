using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class SaveSystem
{
	private static SaveData saveData = new SaveData();

	[System.Serializable]
	public struct SaveData
	{
		public PlayerSaveData PlayerData;
		public PlayerUnlockData UnlockData;
		public WWitchSaveData WWitchData;
		public BWitchSaveData BWitchData;
		public RWitchSaveData RWitchData;
		public SceneSaveData SceneData;
		public CamData CamData;
		public TimerSaveData elapsedTimeData;
		public DeathSaveData deathsData;
	}

	public static string SaveFileName()
	{
		string currentSceneName = GameManager.Instance.sceneData.Data.sceneName;
		string saveFile = Application.persistentDataPath + "/save_" + currentSceneName + ".sav";
		return saveFile;
	}

	public static void Save()
	{
		HandleSave();

		File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
	}

	public static async Task SaveAsync()
	{
		await SaveAsynchornously();
	}

	private static async Task SaveAsynchornously()
	{
		HandleSave();

		await File.WriteAllTextAsync(SaveFileName(), JsonUtility.ToJson(saveData, true));
	}

	private static void HandleSave()
	{
		GameManager.Instance.playerController.Save(ref saveData.PlayerData);
		GameManager.Instance.playerUnlock.Save(ref saveData.UnlockData);
		if (GameManager.Instance.wwitch != null && GameManager.Instance.rwitch != null && GameManager.Instance.bwitch != null)
		{
			GameManager.Instance.wwitch.Save(ref saveData.WWitchData);
			GameManager.Instance.rwitch.Save(ref saveData.RWitchData);
			GameManager.Instance.bwitch.Save(ref saveData.BWitchData);
		}
		GameManager.Instance.sceneData.Save(ref saveData.SceneData);
		GameManager.Instance.currentCam.Save(ref saveData.CamData);
		GameManager.Instance.timer.Save(ref saveData.elapsedTimeData);
		GameManager.Instance.deathCounter.Save(ref saveData.deathsData);
	}

	public static void Load()
	{
		string saveContent = File.ReadAllText(SaveFileName());

		saveData = JsonUtility.FromJson<SaveData>(saveContent);
		HandleLoad();
	}

	public static async Task LoadAsync()
	{
		string saveContent = File.ReadAllText(SaveFileName());

		saveData = JsonUtility.FromJson<SaveData>(saveContent);
		await HandleLoadAsync();
	}

	public static async Task LoadSelectedAsync(string saveFile)
	{
		if (File.Exists(saveFile))
		{
			string saveContent = File.ReadAllText(saveFile);
			saveData = JsonUtility.FromJson<SaveData>(saveContent);
			await HandleLoadAsync();
		}
		else
		{
			Debug.Log($"Save file does not exist: {saveFile}");
		}
	}

	private static async Task HandleLoadAsync()
	{
		await GameManager.Instance.sceneData.LoadAsync(saveData.SceneData);
		await GameManager.Instance.sceneData.Wait4Scene();

		await Task.Yield();

		GameManager.Instance.playerController.Load(saveData.PlayerData);
		GameManager.Instance.playerUnlock.Load(saveData.UnlockData);
		GameManager.Instance.currentCam.Load(saveData.CamData);
		GameManager.Instance.timer.Load(saveData.elapsedTimeData);
		GameManager.Instance.deathCounter.Load(saveData.deathsData);

		if (GameManager.Instance.wwitch != null && GameManager.Instance.rwitch != null && GameManager.Instance.bwitch != null)
		{
			GameManager.Instance.wwitch.Load(saveData.WWitchData);
			GameManager.Instance.rwitch.Load(saveData.RWitchData);
			GameManager.Instance.bwitch.Load(saveData.BWitchData);
		}
	}

	public static void HandleLoad()
	{
		GameManager.Instance.playerController.Load(saveData.PlayerData);
		GameManager.Instance.playerUnlock.Load(saveData.UnlockData);
		GameManager.Instance.sceneData.Load(saveData.SceneData);
	}

	public static SaveData LoadLevelData(string saveFilePath)
	{
		if (File.Exists(saveFilePath))
		{
			string saveContent = File.ReadAllText(saveFilePath);
			return JsonUtility.FromJson<SaveData>(saveContent);
		}
		return new SaveData();
	}
}
