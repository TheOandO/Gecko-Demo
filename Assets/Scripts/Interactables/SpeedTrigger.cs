using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpeedTrigger : MonoBehaviour
{
	private GameObject player;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject == player)
		{
			PlayerController pc = player.GetComponent<PlayerController>();

			pc.walkSpeed = 5;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject == player)
		{
			PlayerController pc = player.GetComponent<PlayerController>();

			pc.walkSpeed = 9;
		}
	}
}
