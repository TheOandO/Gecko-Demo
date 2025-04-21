using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SaveTrigger : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			gameObject.SetActive(false);
			GameManager.Instance.SaveAsync();
		}
	}
}
