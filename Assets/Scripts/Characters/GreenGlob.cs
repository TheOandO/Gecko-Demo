using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenGlob : MonoBehaviour
{
	private Animator animator;

	[SerializeField] private float floatAmplitude = 0.5f;
	[SerializeField] private float floatSpeed = 1f;

	private Vector3 startPosition;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		startPosition = transform.position;
	}

	void Update()
	{
		float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

		transform.position = new Vector3(startPosition.x, newY, startPosition.z);
	}
}
