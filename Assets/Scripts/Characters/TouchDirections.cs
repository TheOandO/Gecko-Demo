using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class TouchDirections : MonoBehaviour
{
    public ContactFilter2D castFilter;
    public float groundDistance = 0.05f;
    public float ceilingDistance = 0.5f;
	public float wallDistance = 0.5f;

	public bool ledgeDetected;

	CapsuleCollider2D touchColl;
    Animator animator;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];

	[SerializeField]
    private bool _isGrounded;

	public bool IsGrounded { get {
        return _isGrounded;
        } private set {
			_isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, value);
		} 
    }

	[SerializeField]
	private bool _isOnWall;

	public bool IsOnWall
	{
		get
		{
			return _isOnWall;
		}
		private set
		{
			_isOnWall = value;
			animator.SetBool(AnimationStrings.isOnWall, value);
		}
	}

	[SerializeField]
	private bool _isOnCeiling;
	private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
	public bool IsOnCeiling
	{
		get
		{
			return _isOnCeiling;
		}
		private set
		{
			_isOnCeiling = value;
			animator.SetBool(AnimationStrings.isOnCeiling, value);
		}
	}

	[SerializeField]
	private bool isOnSlope;

	public bool IsOnSlope
	{
		get
		{
			return isOnSlope;
		}
		private set
		{
			isOnSlope = value;
			animator.SetBool(AnimationStrings.isOnSlope, value);
		}
	}

	private void Awake()
	{
		touchColl = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
	}

    void FixedUpdate()
    {
		IsGrounded = touchColl.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsOnWall = touchColl.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;
        IsOnCeiling = touchColl.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
	}
}
