using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraManager : MonoBehaviour
{
	public static CameraManager instance;

	[SerializeField] private CinemachineVirtualCamera[] allCams;

	[Header("For Lerping Y Damp")]
	[SerializeField] private float fallPanAmount = 0.25f;
	[SerializeField] private float fallYPanTime = 0.35f;
	public float fallSpeedYDampChangeThreshold = -15f;

	public bool IsLerpYDamping { get; private set; }
	public bool LerpedFromFalling {  get; set; }

	private Coroutine lerpYPan;
	private Coroutine panCam;

	private CinemachineVirtualCamera currentCam;
	private CinemachineFramingTransposer framingTransposer;

	private Vector2 startingTrackedObjOffset;

	private float normYPanAmount;

	private void Awake()
	{
		if (instance == null)
			instance = this;

		for (int i = 0; i < allCams.Length; i++)
		{
			if (allCams[i].enabled)
			{
				//Set current active cam
				currentCam = allCams[i];

				//Set framing transposer
				framingTransposer = currentCam.GetCinemachineComponent<CinemachineFramingTransposer>();
			}
		}

		normYPanAmount = framingTransposer.m_YDamping;
	}

	private void Start()
	{
		GameManager.Instance.currentCam = this;
	}

	public void LerpYDamping(bool isFalling)
	{
		lerpYPan = StartCoroutine(LerpYAction(isFalling));
	}

	private IEnumerator LerpYAction(bool isFalling)
	{
		IsLerpYDamping = true;

		//Find starting damp amount
		float startDampAmount = framingTransposer.m_YDamping;
		float endDampAmount = 0f;

		//Determine end damp amount
		if (isFalling)
		{
			endDampAmount = fallPanAmount;
			LerpedFromFalling = true;
		}
        else
        {
			endDampAmount = normYPanAmount;
        }

		//Lerp pan amount
		float elapsedTime = 0f;
		while (elapsedTime < fallYPanTime)
		{
			elapsedTime += Time.deltaTime;

			float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / fallYPanTime));
			framingTransposer.m_YDamping = lerpedPanAmount;

			yield return null;
		}

		IsLerpYDamping = false;

		startingTrackedObjOffset = framingTransposer.m_TrackedObjectOffset;
    }

	public void PanCamOnContact(float panDistance, float panTime, PanDirections panDirection, bool panToStartingPos)
	{
		panCam = StartCoroutine(PanCam(panDistance, panTime, panDirection, panToStartingPos));
	}

	private IEnumerator PanCam(float panDistance, float panTime, PanDirections panDirection, bool panToStartingPos)
	{
		Vector2 endPos = Vector2.zero;
		Vector2 startPos = Vector2.zero;

		if (!panToStartingPos)
		{
			switch (panDirection)
			{
				case PanDirections.Up:
					endPos = Vector2.up;
					break;
				case PanDirections.Down:
					endPos = Vector2.down;
					break;
				case PanDirections.Left:
					endPos = Vector2.left;
					break;
				case PanDirections.Right:
					endPos = Vector2.right;
					break;
				default:
					break;
			}

			endPos *= panDistance;

			startPos = startingTrackedObjOffset;

			endPos += startPos;
		}
		else
		{
			startPos = framingTransposer.m_TrackedObjectOffset;
			endPos = startingTrackedObjOffset;
		}

		float elapsedTime = 0f;
		while (elapsedTime < panTime)
		{
			elapsedTime += Time.deltaTime;

			Vector3 panLerp = Vector3.Lerp(startPos, endPos, (elapsedTime / panTime));
			framingTransposer.m_TrackedObjectOffset = panLerp;

			yield return null;
		}
	}
	public void SwapCams(CinemachineVirtualCamera camFromLeft, CinemachineVirtualCamera camFromRight, Vector2 triggerExitDir)
	{
		//If current cam is left-sided
		if (currentCam == camFromLeft && triggerExitDir.x > 0f)
		{
			camFromRight.enabled = true;

			camFromLeft.enabled = false;

			currentCam = camFromRight;

			framingTransposer = currentCam.GetCinemachineComponent<CinemachineFramingTransposer>();
		}

		//If current cam is right-sided
		else if (currentCam == camFromRight && triggerExitDir.x < 0f)
		{
			camFromLeft.enabled = true;

			camFromRight.enabled = false;

			currentCam = camFromLeft;

			framingTransposer = currentCam.GetCinemachineComponent<CinemachineFramingTransposer>();
		}

		GOResetManager.Instance.ResetAll();
	}

	#region Save/Load

	public void Save(ref CamData data)
	{
		for (int i = 0; i < allCams.Length; i++)
		{
			if (allCams[i] == currentCam)
			{
				data.currentCamIndex = i;
				break;
			}
		}
	}

	public void Load(CamData data)
	{
		if (data.currentCamIndex >= 0 && data.currentCamIndex < allCams.Length)
		{
			currentCam = allCams[data.currentCamIndex];

			if (currentCam != null)
			{
				foreach (var cam in allCams)
				{
					cam.enabled = false;
				}

				currentCam.enabled = true;

				framingTransposer = currentCam.GetCinemachineComponent<CinemachineFramingTransposer>();
			}
		}
		else
		{
			Debug.LogError("Invalid camera index loaded.");
		}
	}

	#endregion
}

[System.Serializable]
public struct CamData
{
	public int currentCamIndex;
}
