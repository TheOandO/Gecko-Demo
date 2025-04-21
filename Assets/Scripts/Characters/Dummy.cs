using System.Collections;
using UnityEngine;
using static Dummy;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchDirections), typeof(Damagable))]
public class Dummy : MonoBehaviour
{
	public DetectionZone attackZone;
	public DetectionZone cliffDetectZone;
	public float walkStopRate = 0.05f;
    public float walkAccelaration = 3f;
	public float maxSpeed = 3f;

	private Vector2 initialPosition;

	Rigidbody2D rb;
	Animator animator;
	TouchDirections touchDirections;
	Damagable dmgable;

	public enum WalkableDirection { Left, Right}

	private WalkableDirection _walkDirection;
	private Vector2 walkDirectionVector = Vector2.left;

	public WalkableDirection WalkDirection {  
		get { return _walkDirection; } 
		set { 
			if (_walkDirection != value)
			{
				gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

				if (value == WalkableDirection.Right) 
				{
					walkDirectionVector = Vector2.right;
				} else if (value == WalkableDirection.Left)
				{
					walkDirectionVector = Vector2.left;
				}
			}
			_walkDirection = value;
		} 
	}

	public bool _hasTarget = false;

	public bool HasTarget { get
		{
			return _hasTarget;
		} private set { 
			_hasTarget = value;
			animator.SetBool(AnimationStrings.hasTarget, value);
		} 
	}

	public bool _canMove = false;
	public bool CanMove
	{
		get
		{
			return animator.GetBool(AnimationStrings.canMove);
		}
		private set
		{
			_canMove = value;
			animator.SetBool(AnimationStrings.canMove, value);
		}
	}

	public float AttackCooldown { get { 
			return animator.GetFloat(AnimationStrings.attackCooldown);
		}
		private set { 
			animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
		} }

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		touchDirections = GetComponent<TouchDirections>();
		animator = GetComponent<Animator>();
		dmgable = GetComponent<Damagable>();

		initialPosition = transform.position;
	}

	void Update()
	{
		HasTarget = attackZone.detectedColl.Count > 0;
		if (AttackCooldown > 0)
		{
			AttackCooldown -= Time.deltaTime;
		}
	}

	private void FixedUpdate()
	{
		if (touchDirections.IsGrounded && touchDirections.IsOnWall)
		{
			FlipDirection();
		}

		if(!dmgable.LockVelocity)
		{
			if (CanMove && touchDirections.IsGrounded)
			{
				rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + (walkAccelaration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed), rb.velocity.y);
			}
			else
			{
				rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
			}
		}
	}

	public float stopDuration = 1f;

	private void FlipDirection()
	{
		if (WalkDirection == WalkableDirection.Right)
		{
			WalkDirection = WalkableDirection.Left;
		} else if (WalkDirection == WalkableDirection.Left)
		{
			WalkDirection = WalkableDirection.Right;
		}
		else
		{
			Debug.LogError("Not set walk direction");
		}
	}

	public void OnHit(int dmg, Vector2 knockback)
	{
		rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
	}

	public void OnCliffDetect()
	{
		if (gameObject.activeInHierarchy)
		{
			StartCoroutine(StandStillThenFlip());
		}
	}

	private IEnumerator StandStillThenFlip()
	{
		rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);

		_canMove = false;
		animator.SetBool(AnimationStrings.canMove, _canMove);

		yield return new WaitForSeconds(stopDuration);

		_canMove = true;
		animator.SetBool(AnimationStrings.canMove, _canMove);

		FlipDirection();
	}

	public void ResetDummy()
	{
		transform.position = initialPosition;

		dmgable.IsAlive = true;
		animator.SetBool(AnimationStrings.canMove, true);
	}
}
