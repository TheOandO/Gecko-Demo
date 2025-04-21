using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OpenableObject: InteractableItem
{
	[SerializeField] private SpriteRenderer sr;
	[SerializeField] private Animator uiAnimator;

	public Sprite open;
	public Sprite closed;

	private bool isOpen = false;

	public override void Interact()
	{
		if (uiAnimator == null)
		{
			if (isOpen)
			{
				sr.sprite = closed;
			}
			else
			{
				sr.sprite = open;
			}
		}
		else
		{
			if (isOpen)
			{
				uiAnimator.SetTrigger("close");
			}
			else
			{
				uiAnimator.SetTrigger("open");
			}
		}

		isOpen = !isOpen;
	}
}
