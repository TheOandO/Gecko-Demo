using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
	public UnityEvent noCollidersRemain;

    public List<Collider2D> detectedColl = new List<Collider2D>();
    Collider2D col;

	private void Awake()
	{
		col = GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		detectedColl.Add(collision);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		detectedColl.Remove(collision);

		if (detectedColl.Count <= 0 )
		{
			noCollidersRemain.Invoke();
		}
	}

	public void ResetZone()
	{
		detectedColl.Clear();
		noCollidersRemain.Invoke();
	}
}
