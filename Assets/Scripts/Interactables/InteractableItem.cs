using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class InteractableItem : MonoBehaviour
{
	private void Reset()
	{
		GetComponent<BoxCollider2D>().isTrigger = true;
	}

	public abstract void Interact();

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			collision.GetComponent<PlayerInput>().actions["Jump"].Disable();
			collision.GetComponentInChildren<PlayerInteract>().OpenInteractIcon();
		}	
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			collision.GetComponent<PlayerInput>().actions["Jump"].Enable();
			collision.GetComponentInChildren<PlayerInteract>().CloseInteractIcon();
		}
	}
}
