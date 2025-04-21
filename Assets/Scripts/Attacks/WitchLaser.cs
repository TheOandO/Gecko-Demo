using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchLaser : MonoBehaviour
{
	[SerializeField] private float defDistanceRay = 100;
	public float maxLaserDuration = 0.2f;
	private float laserLerpDuration = 0.4f;
	private float timer = 0f;
	private float lineWidth;

	private LineRenderer lineRenderer;
	[SerializeField]private BoxCollider2D boxCollider;

	Transform m_Transform;

	private Vector2 laserDirection;

	private void Awake()
	{
		m_Transform = GetComponent<Transform>();
		lineRenderer = GetComponentInChildren<LineRenderer>();

		lineWidth = lineRenderer.startWidth;
		lineRenderer.enabled = false;
		boxCollider.enabled = false;
	}

	private void Update()
	{
		if (lineRenderer.enabled)
		{
			timer += Time.deltaTime;

			AdjustLineHeight();
		}


		if (timer > maxLaserDuration)
		{
			DestroyLaser();
		}
	}

	public void ShootLaser(Transform laserFirePoint)
	{
		lineRenderer.startWidth = lineWidth;
		lineRenderer.endWidth = lineWidth;

		laserDirection = (laserFirePoint.position - m_Transform.position).normalized;

		RaycastHit2D hit = Physics2D.Raycast(m_Transform.position, laserDirection);

		if (hit)
		{
			Draw2DRay(laserFirePoint.position, hit.point);
		}
		else
		{
			Draw2DRay(m_Transform.position, m_Transform.position + (Vector3)(laserDirection * defDistanceRay));
		}
	}

	void Draw2DRay(Vector2 startPos, Vector2 endPos)
	{
		lineRenderer.enabled = true;
		lineRenderer.SetPosition(0, startPos);
		lineRenderer.SetPosition(1, endPos);

		AdjustCollider(startPos, endPos);
	}

	void AdjustCollider(Vector2 startPos, Vector2 endPos)
	{
		Vector2 direction = endPos - startPos;
		float distance = direction.magnitude;

		boxCollider.size = new Vector2(distance, lineWidth);

		Vector2 midpoint = (startPos + endPos) / 2;

		boxCollider.transform.position = midpoint;

		boxCollider.offset = Vector2.zero;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		boxCollider.transform.rotation = Quaternion.Euler(0, 0, angle);

		boxCollider.enabled = true;
	}

	void AdjustLineHeight()
	{
		float currentWidth = Mathf.Lerp(lineWidth, 0, timer / laserLerpDuration);
		lineRenderer.startWidth = currentWidth;
		lineRenderer.endWidth = currentWidth;

		AdjustColliderHeight(currentWidth);
	}

	void AdjustColliderHeight(float currentLineWidth)
	{
		boxCollider.size = new Vector2(boxCollider.size.x, currentLineWidth);
	}

	public void DestroyLaser()
	{
		timer = 0f;

		lineRenderer.enabled = false;
		boxCollider.enabled = false;
	}
}
