using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
	[SerializeField] ParticleSystem moveParticle;
	[SerializeField] ParticleSystem fallParticle;
	[SerializeField] ParticleSystem slideParticle;
	[SerializeField] ParticleSystem dieParticle;

	[Range(0, 10)]
	[SerializeField] int occurAfterVelocity;

	[Range(0, 0.2f)]
	[SerializeField] float dustFormationPeriod;

	[SerializeField] Rigidbody2D playerRb;

	bool isOnGround;

	float counter;

	private void Awake()
	{
		dieParticle.Stop();
	}
	private void Update()
	{
		counter += Time.deltaTime;

		if (Mathf.Abs(playerRb.velocity.x) > occurAfterVelocity && isOnGround)
		{
			if (counter > dustFormationPeriod)
			{
				moveParticle.Play();
				counter = 0;
			}
		}
	}

	public void PlaySlideParticle()
	{
		slideParticle.Play();
	}

	public void PlayDieParticle()
	{
		var main = dieParticle.main;
		var shape = dieParticle.shape;

		ParticleSystem.MinMaxCurve currentStartSpeed = main.startSpeed;

		if (currentStartSpeed.mode == ParticleSystemCurveMode.Constant && currentStartSpeed.constant < 0)
		{
			currentStartSpeed.constant = Mathf.Abs(currentStartSpeed.constant); // Set it to positive
		}

		main.startSpeed = currentStartSpeed;
		main.startLifetime = 20;
		shape.radius = 0.3f;
		shape.radiusThickness = 0;

		dieParticle.Play();
	}
	public void StopDieParticle()
	{
		dieParticle.Stop();
	}

	public void ReverseDieParticle()
	{
		var main = dieParticle.main;
		var shape = dieParticle.shape;

		ParticleSystem.MinMaxCurve currentStartSpeed = main.startSpeed;

		if (currentStartSpeed.mode == ParticleSystemCurveMode.Constant)
		{
			currentStartSpeed.constant = -Mathf.Abs(currentStartSpeed.constant);
		}

		main.startSpeed = currentStartSpeed;
		main.startLifetime = 3.5f;
		shape.radius = 8f;
		shape.radiusThickness = 1;

		dieParticle.Play();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Ground"))
		{
			fallParticle.Play();
			isOnGround = true;
		}

	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Ground"))
			isOnGround = false;
	}
}
