using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObj : MonoBehaviour
{
	[SerializeField] private Transform playerTrans;

	[Header("Flip Rotation Stats")]
	[SerializeField] private float flipRotationTime = 0.5f;

	private Coroutine turnCoroutine;

	private PlayerController playerController;

	private bool isFacingRight;

	private void Awake()
	{
		playerController = playerTrans.gameObject.GetComponent<PlayerController>();

		isFacingRight = playerController.IsFacingRight;
	}

	private void Update()
	{
		transform.position = playerTrans.position;
	}

	public void CallTurn()
	{
		turnCoroutine = StartCoroutine(FlipYLerp());
	}

	private IEnumerator FlipYLerp()
	{
		Vector3 startRotation = transform.localScale;
		Vector3 endRotationAmount = DetermineEndScale();

		float elapsedTime = 0f;
		while (elapsedTime < flipRotationTime)
		{
			elapsedTime += Time.deltaTime;

			transform.localScale = Vector3.Lerp(startRotation, endRotationAmount, elapsedTime / flipRotationTime);

			yield return null;
		}
	}
	private Vector3 DetermineEndScale()
	{
		// Toggle the facing direction.
		isFacingRight = !isFacingRight;

		// If facing right, set scale to positive; if facing left, set scale to negative.
		Vector3 newScale = transform.localScale;
		newScale.x = isFacingRight ? -Mathf.Abs(newScale.x) : Mathf.Abs(newScale.x);
		return newScale;
	}
}
