using Cinemachine;
using System.Collections;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class CameraCtrlTrigger : MonoBehaviour
{
	public CustomInspectorObjects customInspectorObjects;

	private Collider2D coll;

	private void Start()
	{
		coll = GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			if (customInspectorObjects.panCamOnContact)
			{
				//Pan Camera
				CameraManager.instance.PanCamOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
			}

		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			Vector2 exitDirection = (collision.transform.position - coll.bounds.center).normalized;

			if (customInspectorObjects.swapCams)
			{
				//if (customInspectorObjects.camBefore != CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera)
				//{
				//	customInspectorObjects.camBefore = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;
				//}

				if (customInspectorObjects.camBefore == null)
				{
					customInspectorObjects.camBefore = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;
				}

				if (customInspectorObjects.camAfter == null)
				{
					customInspectorObjects.camAfter = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;
				}

				if (customInspectorObjects.camBefore == customInspectorObjects.camAfter)
				{
					return;
				}

				CameraManager.instance.SwapCams(customInspectorObjects.camBefore, customInspectorObjects.camAfter, exitDirection);
			}

			if (customInspectorObjects.panCamOnContact)
			{
				CameraManager.instance.PanCamOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, true);
			}
		}
	}
}

[System.Serializable]
public class CustomInspectorObjects
{
	public bool swapCams = false;
	public bool panCamOnContact = false;

	[HideInInspector] public CinemachineVirtualCamera camBefore;
	[HideInInspector] public CinemachineVirtualCamera camAfter;

	[HideInInspector] public PanDirections panDirection;
	[HideInInspector] public float panDistance = 3f;
	[HideInInspector] public float panTime = 0.35f;
}

public enum PanDirections
{
	Up,
	Down,
	Left,
	Right
}


#if UNITY_EDITOR
[CustomEditor(typeof(CameraCtrlTrigger))]
public class MyScriptEditor : Editor
{
	CameraCtrlTrigger cameraCtrlTrigger;

	private void OnEnable()
	{
		cameraCtrlTrigger = (CameraCtrlTrigger)target;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (cameraCtrlTrigger.customInspectorObjects.swapCams)
		{
			cameraCtrlTrigger.customInspectorObjects.camBefore = EditorGUILayout.ObjectField("Cam Before", cameraCtrlTrigger.customInspectorObjects.camBefore, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

			cameraCtrlTrigger.customInspectorObjects.camAfter = EditorGUILayout.ObjectField("Cam After", cameraCtrlTrigger.customInspectorObjects.camAfter, typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
		}

		if (cameraCtrlTrigger.customInspectorObjects.panCamOnContact)
		{
			cameraCtrlTrigger.customInspectorObjects.panDirection = (PanDirections)EditorGUILayout.EnumPopup("Cam Pan Direction", cameraCtrlTrigger.customInspectorObjects.panDirection);

			cameraCtrlTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraCtrlTrigger.customInspectorObjects.panDistance);
			cameraCtrlTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraCtrlTrigger.customInspectorObjects.panTime);
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(cameraCtrlTrigger);
		}
	}
}
#endif
