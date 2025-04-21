using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TalkableObject: InteractableItem
{
	[SerializeField] private Animator uiAnimator;
	[SerializeField] private GameObject interactCard;

	Animator animator;

	[SerializeField] private float interactionCooldown = 0.5f;

	private bool isTalking = false;
	private bool isOnCooldown = false;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	public override void Interact()
	{
		if (isOnCooldown) return;

		if (isTalking)
		{
			uiAnimator.SetTrigger("close");

			interactCard.SetActive(false);
		}
		else
		{
			AudioManager.Instance.PlaySFX(AudioManager.Instance.cat);
			uiAnimator.SetTrigger("open");
			interactCard.SetActive(true);
		}

		animator.SetTrigger("point");
		isTalking = !isTalking;
		StartCoroutine(StartCooldown());
	}

	private IEnumerator StartCooldown()
	{
		isOnCooldown = true;
		yield return new WaitForSeconds(interactionCooldown);
		isOnCooldown = false;
	}
}
