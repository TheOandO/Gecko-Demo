using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
	[Header("Interact")]
	[SerializeField] private GameObject interactIcon;
	private Vector2 boxSize = new Vector2(0.1f, 1f);

	public void OpenInteractIcon()
	{
		interactIcon.SetActive(true);
	}

	public void CloseInteractIcon()
	{
		interactIcon.SetActive(false);
	}

	public void CheckInteraction()
	{
		RaycastHit2D[] hit = Physics2D.BoxCastAll(transform.position, boxSize, 0, Vector2.zero);

		if (hit.Length > 0)
		{
			foreach (RaycastHit2D rc in hit)
			{
				if (rc.IsInteractable())
				{
					rc.Interact();
					return;
				}
			}
		}
	}
}
