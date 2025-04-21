using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableHealthBar : MonoBehaviour
{
	public GameObject healthBar;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		PlayerPrefs.SetInt("BossEncountered", 0);
		PlayerPrefs.Save();

		healthBar.SetActive(false);
	}
}
