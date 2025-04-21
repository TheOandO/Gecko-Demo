using System.Collections;
using UnityEngine;

public class DashPickup : MonoBehaviour
{
	public float dashIncr = 1;
	public float pickupCooldown = 3f;

	private bool isAvailable = true;
	public bool IsAvailable
	{
		get
		{
			return animator.GetBool("active");
		}
		private set
		{
			isAvailable = value;
			animator.SetBool("active", value);
		}
	}
	Animator animator;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isAvailable && collision.GetComponent<PlayerController>())
		{
			collision.GetComponent<PlayerController>().RefreshDash(dashIncr);
			IsAvailable = false;

			StartCoroutine(PickupCooldown());
		}
	}

	private IEnumerator PickupCooldown()
	{
		yield return new WaitForSeconds(pickupCooldown);
		IsAvailable = true;
	}

	public void ResetPickup()
	{
		StopAllCoroutines();
		IsAvailable = true;
	}
}
