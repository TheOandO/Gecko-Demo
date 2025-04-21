using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchSpikes : MonoBehaviour
{
	Animator animator1;
	[SerializeField]Animator animator2;
	BoxCollider2D boxCollider;

	[SerializeField] private float targetOffset = 1.0f;
	[SerializeField] private float targetHeight = 1.4f;
	[SerializeField] private float increaseDuration = 0.5f;
	private float initialHeight;
	private float initialOffset;

	public bool isActive = false;

	private void Awake()
	{
		animator1 = GetComponent<Animator>();
		boxCollider = GetComponent<BoxCollider2D>();

		initialHeight = boxCollider.size.y;
		initialOffset = boxCollider.offset.y;

	}

	private void Update()
	{
		if (isActive)
		{
			EnableSpikes();
		}
		else
		{
			DisableSpikes();
		}
	}

	public void EnableSpikes()
	{
		boxCollider.enabled = true;

		animator1.SetBool("isActive", true);
		animator2.SetBool("isActive", true);
	}

	public void DisableSpikes()
	{
		animator1.SetBool("isActive", false);
		animator2.SetBool("isActive", false);

		boxCollider.enabled = false;
	}

	public void ChangeCollider()
	{
		StartCoroutine(IncreaseColliderHeight());
	}

	public void ResetCollider()
	{
		boxCollider.size = new Vector2(boxCollider.size.x, initialHeight);
		boxCollider.offset = new Vector2(boxCollider.offset.x, initialOffset);
	}

	private IEnumerator IncreaseColliderHeight()
	{
		boxCollider.enabled = true;

		float elapsedTime = 0f;

		while (elapsedTime < increaseDuration)
		{
			elapsedTime += Time.deltaTime;

			float newHeight = Mathf.Lerp(initialHeight, targetHeight, elapsedTime / increaseDuration);
			float newOffset = Mathf.Lerp(initialOffset, targetOffset, elapsedTime / increaseDuration);

			boxCollider.size = new Vector2(boxCollider.size.x, newHeight);
			boxCollider.offset = new Vector2(boxCollider.offset.x, newOffset);

			yield return null;
		}

		boxCollider.size = new Vector2(boxCollider.size.x, targetHeight);
	}
}
