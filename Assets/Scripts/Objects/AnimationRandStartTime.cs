using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRandStartTime : MonoBehaviour
{
	private Animator animator;

	private void Start()
	{
		animator = GetComponent<Animator>();

		float randomStartTime = Random.Range(0f, animator.GetCurrentAnimatorStateInfo(0).length);

		animator.Play(0, -1, randomStartTime);
	}
}
