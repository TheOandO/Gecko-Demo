using Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
	public static CameraShakeManager instance;

	[SerializeField] private float shakeForce = 1f;

	private void Awake()
	{
		if (instance == null)
			instance = this;
	}

	public void Shake(CinemachineImpulseSource impulseSource)
	{
		impulseSource.GenerateImpulseWithForce(shakeForce);
	}
}
