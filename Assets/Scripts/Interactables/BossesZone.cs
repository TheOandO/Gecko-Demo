using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossesZone : MonoBehaviour
{
	public UnityEvent onTilemapEnabled;
	public UnityEvent onTilemapDisabled;

	[SerializeField] private List<Collider2D> playerColliders = new List<Collider2D>();
	[SerializeField] private List<Collider2D> bossColliders = new List<Collider2D>();

	private Collider2D col;

	public Tilemap tilemap;

	private void Awake()
	{
		col = GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Track player
		if (collision.GetComponent<PlayerController>() != null)
		{
			if (!playerColliders.Contains(collision))
				playerColliders.Add(collision);
		}

		// Track bosses
		if (collision.GetComponent<BossBehavior>() != null) 
		{
			if (!bossColliders.Contains(collision))
				bossColliders.Add(collision);
		}

		UpdateTilemapStatus();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		// Remove player
		if (collision.GetComponent<PlayerController>() != null)
		{
			playerColliders.Remove(collision);
		}

		// Remove bosses
		if (collision.GetComponent<BossBehavior>() != null)
		{
			bossColliders.Remove(collision);
		}

		UpdateTilemapStatus();
	}

	private void UpdateTilemapStatus()
	{
		playerColliders.RemoveAll(item => item == null);
		bossColliders.RemoveAll(item => item == null);

		if (playerColliders.Count > 0 && bossColliders.Count > 0)
		{
			EnableTilemap();
			onTilemapEnabled.Invoke();
		}
		else if (playerColliders.Count > 0 && bossColliders.Count == 0)
		{
			DisableTilemap();
			onTilemapDisabled.Invoke();
		}
	}

	private void OnEnable()
	{
		onTilemapEnabled.AddListener(EnableTilemap);
		onTilemapDisabled.AddListener(DisableTilemap);
	}

	private void OnDisable()
	{
		onTilemapEnabled.RemoveListener(EnableTilemap);
		onTilemapDisabled.RemoveListener(DisableTilemap);
	}

	private void EnableTilemap()
	{
		if (tilemap != null)
		{
			tilemap.gameObject.SetActive(true);
		}
	}

	private void DisableTilemap()
	{
		if (tilemap != null)
		{
			tilemap.gameObject.SetActive(false);
		}
	}
}
