using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    Vector2 checkpointPos;

    Damagable dmgable;
    Rigidbody2D rb;
    Animator animator;
	Collider2D[] colliders;
	ParticleController particleController;

	private void Awake()
	{
        rb = GetComponent<Rigidbody2D>();
		colliders = GetComponents<Collider2D>();
		particleController = GetComponentInChildren<ParticleController>();
	}

	void Start()
    {
        dmgable = GetComponent<Damagable>();
		checkpointPos = transform.position;
    }


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Obstacle") && dmgable.IsAlive)
		{
			Die();
		}
	}

	public void UpdateCheckpoint(Vector2 pos)
    {
        checkpointPos = pos;
	}

	public void Die()
	{
		dmgable.Hit(100, new Vector2(10, 30));

		foreach (Collider2D col in colliders)
		{
			col.enabled = false;
		}

		DeathCounterManager.Instance.IncrementDeathCount();

		animator = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Animator>();

		particleController.PlayDieParticle();
		StartCoroutine(Respawn(1f));
	}

    IEnumerator Respawn(float duration)
    {
		//Time.timeScale = 0;
		yield return new WaitForSeconds(0.2f);
		animator.SetTrigger(AnimationStrings.endTrigger);

		yield return new WaitForSeconds(duration);
		animator.SetTrigger(AnimationStrings.startTrigger);

		GOResetManager.Instance.ResetAll();
		GOResetManager.Instance.ResetBosses();
		particleController.ReverseDieParticle();

		foreach (Collider2D col in colliders)
		{
			col.enabled = true;
		}

		transform.position = checkpointPos;
        rb.velocity = Vector2.zero;
		rb.gravityScale = 1;
		dmgable.Health = 1;
        dmgable.IsAlive = true;
		//Time.timeScale = 1;
		yield return new WaitForSeconds(duration);
		particleController.StopDieParticle();
	}
}
