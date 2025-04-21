using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
	public GameObject healthBarUI;

	private List<Damagable> dmgables = new List<Damagable>();
	private int totalHealth;
	private int totalMaxHealth;

	private void Awake()
	{
		GameObject[] bossObjects = GameObject.FindGameObjectsWithTag("Boss");

		foreach (var boss in bossObjects)
		{
			Damagable dmgable = boss.GetComponent<Damagable>();
			if (dmgable != null)
			{
				dmgables.Add(dmgable);
			}
		}

		if (dmgables.Count == 0)
		{
			Debug.LogError("No bosses found");
		}
	}

	void Start()
    {
		UpdateTotalHealth();
		healthSlider.value = CalculatePercentage(totalHealth, totalMaxHealth);

		bool bossEncountered = PlayerPrefs.GetInt("BossEncountered", 0) == 1;
		if (bossEncountered)
		{
			healthBarUI.SetActive(bossEncountered);
		}
		else
		{
			healthBarUI.SetActive(false);
		}
	}

	private void OnEnable()
	{
		foreach (var boss in dmgables)
		{
			boss.healthChanged.AddListener(OnBossHealthChanged);
		}
	}

	private void OnDisable()
	{
		foreach (var boss in dmgables)
		{
			boss.healthChanged.RemoveListener(OnBossHealthChanged);
		}
	}

	private void OnBossHealthChanged(int newHealth, int maxHealth)
	{
		UpdateTotalHealth();
		healthSlider.value = CalculatePercentage(totalHealth, totalMaxHealth);
	}

	private void UpdateTotalHealth()
	{
		totalHealth = 0;
		totalMaxHealth = 0;

		foreach (var boss in dmgables)
		{
			totalHealth += boss.Health;
			totalMaxHealth += boss.MaxHealth;
		}
	}

	private float CalculatePercentage(float currentHealth, float maxHealth)
	{
		return currentHealth / maxHealth;
	}

	public void ShowHealthBar()
	{
		healthBarUI.SetActive(true);
		PlayerPrefs.SetInt("BossEncountered", 1);
		PlayerPrefs.Save();
	}
}
