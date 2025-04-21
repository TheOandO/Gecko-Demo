using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scene Data", fileName = "NewSceneData")]
public class SceneDataSO : ScriptableObject
{
	public string sceneName;
	public int sceneIndex;
	public bool unlockDash;
	public bool unlockRanged;
}
