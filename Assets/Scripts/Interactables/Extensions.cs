using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
	public static bool IsInteractable(this RaycastHit2D hit)
	{
		return hit.transform.GetComponent<InteractableItem>();
	}

	public static void Interact(this RaycastHit2D hit)
	{
		hit.transform.GetComponent<InteractableItem>().Interact();
	}

}
