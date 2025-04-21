using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
	public float fallDelay = 0.8f;
	private float respawnDelay = 3.8f;
	private float fadeDuration = 0.6f;

    public float shakeDuration = 0.5f;
    public float shakeIntensity = 0.08f;

    Rigidbody2D rb;
	Collider2D col;
	List<SpriteRenderer> spriteRenderers;

	private Vector2 initialPosition;
    private Vector3 spriteOriginalLocalPos;
    private Transform spriteParent;
    private bool isFalling = false;

	private void Start()
	{
		spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
		rb = GetComponent<Rigidbody2D>();
		col = rb.GetComponent<Collider2D>();

		initialPosition = transform.position;

        // Create a new GameObject to act as the visual parent
        spriteParent = new GameObject("SpriteParent").transform;
        spriteParent.parent = transform;
        spriteParent.localPosition = Vector3.zero;

        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.transform.parent = spriteParent;
        }

        spriteOriginalLocalPos = spriteParent.localPosition;
    }
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player") && !isFalling)
		{
			StartCoroutine(Fall());
		}
	}

	private IEnumerator Fall()
	{
		StartCoroutine(Shake()); ;

		yield return new WaitForSeconds(fallDelay);
		rb.bodyType = RigidbodyType2D.Dynamic;
		isFalling = true;
		col.enabled = false;
		StartCoroutine(FadeOut());

		yield return new WaitForSeconds(respawnDelay);
        StartCoroutine(Respawn());
	}

    private IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitCircle * shakeIntensity;
            spriteParent.localPosition = spriteOriginalLocalPos + randomOffset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteParent.localPosition = spriteOriginalLocalPos;
    }

    private IEnumerator Respawn()
	{
		StartCoroutine(FadeIn());
		isFalling = false;
		rb.bodyType = RigidbodyType2D.Kinematic;
		rb.velocity = Vector2.zero;
		transform.position = initialPosition;

        yield return new WaitForSeconds(fadeDuration);

        col.enabled = true;

	}

	private IEnumerator FadeOut()
	{
		float elapsedTime = 0f;
		while (elapsedTime < fadeDuration)
		{
			foreach (SpriteRenderer spriteRenderer in spriteRenderers)
			{
				Color color = spriteRenderer.color;
				color.a = Mathf.Lerp(0.5f, 0f, elapsedTime / fadeDuration);
				spriteRenderer.color = color;
			}

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		foreach (SpriteRenderer spriteRenderer in spriteRenderers)
		{
			Color color = spriteRenderer.color;
			color.a = 0f;
			spriteRenderer.color = color;
		}
	}

	private IEnumerator FadeIn()
	{
		float elapsedTime = 0f;
		while (elapsedTime < fadeDuration)
		{
			foreach (SpriteRenderer spriteRenderer in spriteRenderers)
			{
				Color color = spriteRenderer.color;
				color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
				spriteRenderer.color = color;
			}

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		foreach (SpriteRenderer spriteRenderer in spriteRenderers)
		{
			Color color = spriteRenderer.color;
			color.a = 1f;
			spriteRenderer.color = color;
		}
	}

	public void ResetPlatform()
	{
		StopAllCoroutines();
        StartCoroutine(Respawn());
		foreach (SpriteRenderer spriteRenderer in spriteRenderers)
		{
			Color color = spriteRenderer.color;
			color.a = 1f; 
			spriteRenderer.color = color;
		}
	}
}
