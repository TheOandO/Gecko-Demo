using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GOResetManager : MonoBehaviour
{
	public static GOResetManager Instance;

	private List<DetectionZone> dZones;
	private List<FallingPlatform> fallingPlatforms;
	private List<Dummy> enemies;
	private List<FlyEye> flyEyes;
	private List<DashPickup> dashPickup;
	private List<FireBall> fireBalls;
	private List<WitchSlowBall> witchSlowBalls;
	private List<PlayerSpit> playerSpit;
	private List<BossBehavior> bosses;
	private List<MovingPlatform> movingPlatforms;
	private Witch_W wwitch;
	private WitchW_Test wwitchTest;
	private Witch_R rwitch;
	private WitchR_Test rwitchTest;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}

	private void Start()
	{
		fallingPlatforms = new List<FallingPlatform>(FindObjectsOfType<FallingPlatform>());
		dashPickup = new List<DashPickup>(FindObjectsOfType<DashPickup>());
		bosses = new List<BossBehavior>(FindObjectsOfType<BossBehavior>());
		movingPlatforms = new List<MovingPlatform>(FindObjectsOfType<MovingPlatform>());
		dZones = new List<DetectionZone>(FindObjectsOfType<DetectionZone>());
	}

	public void ResetAll()
	{
		ResetAllPlatforms();

        ResetAllEnemies();

		ResetDashPickUp();
		DestroyAllProjectile();
	}

	private void ResetAllPlatforms()
	{
		ResetFallingPlatforms();
		ResetMovingPlatforms();
	}

	private void ResetAllEnemies()
	{
		ResetDummy();
		ResetFlyEye();
		ResetZones();
	}

	private void ResetDashPickUp()
	{
		if (dashPickup != null)
		{
			foreach (var dashPickup in dashPickup)
			{
				dashPickup.ResetPickup();
			}
		}

	}

	private void DestroyAllProjectile()
	{
		fireBalls = new List<FireBall>(FindObjectsOfType<FireBall>());
		witchSlowBalls = new List<WitchSlowBall>(FindObjectsOfType<WitchSlowBall>());
		playerSpit = new List<PlayerSpit>(FindObjectsOfType<PlayerSpit>());

		if (fireBalls != null)
		{
			foreach (var fireBall in fireBalls)
			{
				fireBall.DestroyProjectile();
			}
		}

		if (witchSlowBalls != null)
		{
			foreach (var slowBall in witchSlowBalls)
			{
				slowBall.DestroyProjectile();
			}
		}

		if (playerSpit != null)
		{
			foreach (var spit in playerSpit)
			{
				spit.DestroyProjectile();
			}
		}
	}

	private void ResetFallingPlatforms()
	{
		if (fallingPlatforms != null)
		{
			foreach (var platform in fallingPlatforms)
			{
				platform.ResetPlatform();
			}
		}
	}

	private void ResetMovingPlatforms()
	{
		if (movingPlatforms != null)
		{
			foreach (var platform in movingPlatforms)
			{
				platform.ResetPlatform();
			}
		}
	}

	private void ResetDummy()
	{
		enemies = new List<Dummy>(FindObjectsOfType<Dummy>());

		if (enemies != null)
		{
			foreach (var enemy in enemies)
			{
				enemy.ResetDummy();
			}
		}

	}

	private void ResetFlyEye()
	{
		flyEyes = new List<FlyEye>(FindObjectsOfType<FlyEye>());

		if (flyEyes != null)
		{
			foreach (var flyEye in flyEyes)
			{
				flyEye.ResetFlyEye();
			}
		}

	}

	private void ResetZones()
	{
		if (dZones != null)
		{
			if (bosses == null)
			{
				foreach (var zone in dZones)
				{
					zone.ResetZone();
				}
			}
		}
	}

	public void ResetBosses()
	{
		if (bosses != null)
		{
			foreach (var boss in bosses)
			{
				boss.ResetBoss();
			}

			wwitch = FindObjectOfType<Witch_W>();
			rwitch = FindObjectOfType<Witch_R>();
			wwitchTest = FindObjectOfType<WitchW_Test>();
			rwitchTest = FindObjectOfType<WitchR_Test>();

			if (wwitch != null) wwitch.ResetWWitch();
			if (rwitch != null) rwitch.ResetRWitch();
			if (wwitchTest != null) wwitchTest.ResetWWitch();
			if (rwitchTest != null) rwitchTest.ResetRWitch();
		}

	}
}
