using UnityEngine;
using UnityEngine.Events;

public class Damagable : MonoBehaviour
{
    public UnityEvent<int, Vector2> dmgableHit;
    public UnityEvent dmgableDeath;
    public UnityEvent<int, int> healthChanged;

    Animator animator;

    [SerializeField]
    private int _maxHealth = 100;

    public int MaxHealth {  get { return _maxHealth; } set {  _maxHealth = value; } }

    [SerializeField]
    private int _health = 100;

    public int Health { get { return _health; } set { 
            _health = value; 
            healthChanged?.Invoke(_health, MaxHealth);

            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }
    [SerializeField]
    private bool _isAlive = true;

	[SerializeField]
	private bool isInvincible = false;

	private float timeSinceHit = 0;
	public float invincibilityTime = 0.25f;

	public bool IsAlive { get {
        return _isAlive;
        } set {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            if (value == false)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.death);
                dmgableDeath.Invoke();
            }
        } 
    }

	public bool LockVelocity
	{
		get
		{
			return animator.GetBool(AnimationStrings.lockVelocity);
		}
		set
		{
			animator.SetBool(AnimationStrings.lockVelocity, value);
		}
	}

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                isInvincible = false ;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
	}

	public bool Hit(int dmg, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= dmg;
            isInvincible = true;

            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            dmgableHit?.Invoke(dmg, knockback);

            return true;
        }
        else
        {
            return false;
        }
    }

  //  public bool Heal(int healthRestore)
  //  {
  //      if(IsAlive && Health < MaxHealth)
  //      {
		//	int maxHeal = Mathf.Max(MaxHealth - Health, 0);
		//	int realHeal = Mathf.Min(maxHeal, healthRestore);
		//	Health += realHeal;

  //          CharacterEvents.characterHealed.Invoke(gameObject, realHeal);
		//	return true;
		//}

		//return false;
  //  }
}
