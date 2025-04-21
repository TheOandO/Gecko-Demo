using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassVelocityController : MonoBehaviour
{
	[Range(0f, 1f)] public float InfluenceStrength = 0.25f;
	public float EaseInTime = 0.15f;
	public float EaseOutTime = 0.15f;
	public float VelocityThreshold = 5f;

	private int ExternalInfluence = Shader.PropertyToID("_ExternalInfluence");

	public void InfluenceGrass(Material mat, float XVelocity)
	{
		mat.SetFloat(ExternalInfluence, XVelocity);
	}
}
