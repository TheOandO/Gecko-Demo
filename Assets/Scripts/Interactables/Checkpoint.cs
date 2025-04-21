using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	PlayerRespawn pr;
	public Transform respawnPoint;

	private void Awake()
	{
		pr = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRespawn>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			if (respawnPoint != null)
            {
				pr.UpdateCheckpoint(respawnPoint.position);
			}
			else
			{
				pr.UpdateCheckpoint(transform.position);
			}
		}
	}
}
