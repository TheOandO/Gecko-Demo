using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassVelocityTrigger : MonoBehaviour
{
	private GrassVelocityController grassVelocityController;
	private GameObject player;
	private Material mat;
	private Rigidbody2D rb;

	private bool easeInRunning;
	private bool easeOutRunning;

	private int ExternalInfluence = Shader.PropertyToID("_ExternalInfluence");

	private float startXVelocity;
	private float velocityLastFrame;
	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		rb = player.GetComponent<Rigidbody2D>();
		grassVelocityController = GetComponentInParent<GrassVelocityController>();
		mat = GetComponent<SpriteRenderer>().material;
		
		startXVelocity = mat.GetFloat(ExternalInfluence);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject == player)
		{
			if (!easeInRunning && Mathf.Abs(rb.velocity.x) > Mathf.Abs(grassVelocityController.VelocityThreshold))
			{
				StartCoroutine(EaseIn(rb.velocity.x * grassVelocityController.InfluenceStrength));
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject == player && gameObject.activeInHierarchy)
		{
			StartCoroutine(EaseOut());
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject == player)
		{
			if (Mathf.Abs(velocityLastFrame) > Mathf.Abs(grassVelocityController.VelocityThreshold) && Mathf.Abs(rb.velocity.x) < Mathf.Abs(grassVelocityController.VelocityThreshold))
			{
				StartCoroutine(EaseOut());
			}
			else if (Mathf.Abs(velocityLastFrame) < Mathf.Abs(grassVelocityController.VelocityThreshold) && Mathf.Abs(rb.velocity.x) > Mathf.Abs(grassVelocityController.VelocityThreshold))
			{
				StartCoroutine(EaseIn(rb.velocity.x * grassVelocityController.InfluenceStrength));
			}
			else if (!easeInRunning && !easeOutRunning && Mathf.Abs(rb.velocity.x) > Mathf.Abs(grassVelocityController.VelocityThreshold))
			{
				grassVelocityController.InfluenceGrass(mat, rb.velocity.x * grassVelocityController.InfluenceStrength);
			}

			velocityLastFrame = rb.velocity.x;
		}
	}

	private IEnumerator EaseIn(float XVelocity)
	{
		easeInRunning = true;

		float elapsedTime = 0;
		while (elapsedTime < grassVelocityController.EaseInTime)
		{
			elapsedTime += Time.deltaTime;
			float lerpedAmount = Mathf.Lerp(startXVelocity, XVelocity, (elapsedTime / grassVelocityController.EaseInTime));
			grassVelocityController.InfluenceGrass(mat, lerpedAmount);

			yield return null;
		}

		easeInRunning = false;
	}

	private IEnumerator EaseOut()
	{
		easeOutRunning = true;

		float currentXInfluence = mat.GetFloat(ExternalInfluence);
		float elapsedTime = 0;

		while (elapsedTime < grassVelocityController.EaseOutTime)
		{
			elapsedTime += Time.deltaTime;
			float lerpedAmount = Mathf.Lerp(currentXInfluence, startXVelocity, (elapsedTime / grassVelocityController.EaseOutTime));
			grassVelocityController.InfluenceGrass(mat, lerpedAmount);

			yield return null;
		}

		easeOutRunning = false;
	}
}
