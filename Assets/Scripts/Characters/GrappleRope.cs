using System.Collections;
using UnityEngine;

public class GrappleRope : MonoBehaviour
{
	[SerializeField] public Transform target;

	[SerializeField] private int resolution, waveCount, wobbleCount;
	[SerializeField] private float waveSize, animSpeed;

	private LineRenderer line;

	private void Start()
	{
		line = GetComponent<LineRenderer>();
	}

	private IEnumerator AnimateRope(Vector3 targetPos)
	{
		line.positionCount = resolution;
		float angle = LookAtAngle(targetPos - transform.position);


		float percent = 0;
		while (percent <= 1)
		{
			percent += Time.deltaTime * animSpeed;
			SetPoints(targetPos, percent, angle);

			yield return null;
		}
		SetPoints(targetPos, 1, angle);
	}

	// Start rope animation with the specified target
	public void StartRopeAnim(Vector3 targetPos)
	{
		StopAllCoroutines();
		StartCoroutine(AnimateRope(targetPos));
	}

	// Stop rope animation
	public void StopRopeAnim()
	{
		line.positionCount = 0;
		StopAllCoroutines();
	}

	private void SetPoints(Vector3 targetPos, float percent, float angle)
	{
		Vector3 ropeEnd = Vector3.Lerp(transform.position, targetPos, percent);
		float length = Vector2.Distance(transform.position, ropeEnd);

		for (int i = 0; i < resolution; i++)
		{
			float xPos = (float) i / resolution * length;
			float reversePercent = (1 - percent);

			float amplitude = Mathf.Sin(reversePercent * wobbleCount * Mathf.PI) * ((1 - (float) i / resolution) * waveSize);

			float yPos = Mathf.Sin((float) waveCount * i / resolution * 2 * Mathf.PI * reversePercent) * amplitude;

			Vector2 pos = RotatePoint(new Vector2(xPos + transform.position.x, yPos + transform.position.y), transform.position, angle);

			line.SetPosition(i, pos);
		}
	}

	Vector2 RotatePoint(Vector2 point, Vector2 pivot, float angle)
	{
		Vector2 dir = point - pivot;
		dir = Quaternion.Euler(0, 0, angle) * dir;
		point = dir + pivot;
		return point;
	}

	private float LookAtAngle(Vector2 target)
	{
		return Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
	}
}
