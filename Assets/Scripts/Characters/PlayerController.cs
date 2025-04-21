using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchDirections), typeof(Damagable))]
public class PlayerController : MonoBehaviour
{
	[Header("Speed")]
	public float walkSpeed = 5f;
	public float runAccelAmount = 7;
	public float runDeccelAmount = 7;
	public float accelInAir = 7;
	public float deccelInAir = 7;

	[Header("Jump Impulse")]
	public float jumpImpulse = 10f;
	//public float jumpHangTimeThreshold = 4f;
	//public float jumpHangAccelerationMult = 4f;
	//public float jumpHangMaxSpeedMult = 4f;

	[Header("Coyote Timer")]
	private float coyoteTime = 0.15f;
	private float coyoteTimeCounter;

	[Header("Jump Buffer Timer")]
	private float jumpBufferTime = 0.05f;
	private float jumpBufferCounter;

	[Header("Wall Interactions")]
	public bool canWallSlide = true;
	public float slideSpeed = 0.6f;
	private float wallSlideTimer = 0;
	public float wallSlideDelay = 2f;
	private Vector2 wallJumpPower = new Vector2(16f, 16f);
	private bool isWallJumping;
	private float wallJumpDir;
	private float wallJumpCounter;
	private float wallJumpDuration = 0.1f;
	private float wallJumpTime = 0.2f;
	private int lastWallJumpDir;

	[Header("Dash")]
	public bool dashEnabled;
	private bool canDash = true;
	private bool isDashing;
	public float dashPower = 10f;
	public float dashTime = 0.3f;
	public float dashNo = 1;
	public float maxDash = 1;
	public float dashDelay = 0.5f;
	private Vector2 dashDir;

	[Header("Fall Multiplier")]
	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;

	[Header("Camera Stuff")]
	[SerializeField] private GameObject cameraFollowGO;
	private CameraFollowObj cameraFollowObj;
	private float fallSpeedYDampingChangeThreshold;

	[Header("Platform Interactions")]
	[SerializeField] private Vector2 offset1;
	[SerializeField] private Vector2 offset2;

	private Vector2 climbBegunPosition;
	private Vector2 climbOverPosition;

	private bool canGrabLedge = true;
	private bool canClimb;

	public Rigidbody2D platformRb;
	public bool isOnPlatform;

	public bool isOnJumpPad;

	private float gravOriginalScale = 1f;

	public bool doConserveMomentum = true;

	[Header("Ranged Attack")]
	public bool rangedEnabled = false;
	private bool canAttack = true;
	[SerializeField] private float rangedCooldown = 1.5f;

	[Header("Grapple")]
	private LineRenderer line;
	private DistanceJoint2D joint;
	private Node selectedNode;
	public float grappleForce = 10f;
	Vector2 moveInput;

	[Header("Shaders")]
	private int hitEffectBlend = Shader.PropertyToID("_HitEffectBlend"); 
	private int outline = Shader.PropertyToID("_Outline");

	TouchDirections touchDirections;
	Damagable dmgable;
	ParticleController particleController;
	GrappleRope grappleRope;
	Node node;
	LadderMovement ladderMovement;
	GhostTrail ghostTrail;
	PlayerRespawn playerRespawn;
	PlayerInteract playerInteract;
	PlayerInput playerInput;

	Rigidbody2D rb;
	Animator animator;
	Material material;
	SpriteRenderer spriteRenderer;
	Collider2D[] colliders;

	private InputAction horizontalMoveAction;
	private float hMove;
	private float vMove;
	private InputAction verticalMoveAction;
	private CinemachineImpulseSource impulseSource;

	private string originalTag;

	[SerializeField]
	private bool _isMoving = false;
	public bool IsMoving
	{
		get
		{
			return _isMoving;
		}
		private set
		{
			_isMoving = value;
			animator.SetBool(AnimationStrings.isMoving, value);
		}
	}

	[SerializeField]
	private bool _isWallGrabing = false;
	public bool IsWallGrabing
	{
		get
		{
			return _isWallGrabing;
		}
		private set
		{
			_isWallGrabing = value;
			animator.SetBool(AnimationStrings.isWallGrabing, value);
		}
	}

	public bool _isFacingRight = true;
	public bool IsFacingRight
	{
		get
		{
			return _isFacingRight;
		}
		private set
		{
			if (_isFacingRight != value)
			{
				transform.localScale *= new Vector2(-1, 1);
				cameraFollowObj.CallTurn();
			}

			_isFacingRight = value;
		}
	}

	private bool _isGrappling;
	public bool IsGrappling
	{
		get
		{
			return _isGrappling;
		}
		private set
		{
			_isGrappling = value;
			animator.SetBool(AnimationStrings.isGrappling, value);
		}
	}

	public bool CanMove
	{
		get
		{
			return animator.GetBool(AnimationStrings.canMove);
		}
	}
	public bool _canWallJump = false;

	public bool CanWallJump
	{
		get
		{
			return _canWallJump;
		}
		private set
		{
			_canWallJump = value;
			animator.SetBool(AnimationStrings.canWallJump, value);
		}
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		touchDirections = GetComponent<TouchDirections>();
		dmgable = GetComponent<Damagable>();
		playerInput = GetComponent<PlayerInput>();
		particleController = GetComponentInChildren<ParticleController>();
		impulseSource = GetComponent<CinemachineImpulseSource>();
		line = GetComponentInChildren<LineRenderer>();
		joint = GetComponent<DistanceJoint2D>();
		node = FindObjectOfType<Node>();
		grappleRope = GetComponentInChildren<GrappleRope>();
		ladderMovement = GetComponentInChildren<LadderMovement>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		colliders = GetComponents<Collider2D>();
		ghostTrail = GetComponent<GhostTrail>();
		playerRespawn = GetComponentInChildren<PlayerRespawn>();
		playerInteract = GetComponentInChildren<PlayerInteract>();

		material = spriteRenderer.material;

		originalTag = gameObject.tag;

		horizontalMoveAction = playerInput.actions["HorizontalMove"];
		verticalMoveAction = playerInput.actions["VerticalMove"];

		rb.gravityScale = gravOriginalScale;

		line.enabled = false;
		joint.enabled = false;
		selectedNode = null;

		GameManager.Instance.playerController = this;
	}

	private void Start()
	{
		cameraFollowObj = cameraFollowGO.GetComponent<CameraFollowObj>();
		fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampChangeThreshold;
	}

	public bool IsAlive
	{
		get
		{
			return animator.GetBool(AnimationStrings.isAlive);
		}
	}

	private void Update()
	{
		if (IsAlive)
		{
			if (isDashing)
			{
				rb.velocity = Vector2.zero;
				rb.gravityScale = 0;
				canDash = false;

				StartCoroutine(ApplyDashDelay());
				return;
			}

			if (IsGrappling)
			{
				rb.gravityScale = 0;
				joint.distance = 2;
				rb.velocity = Vector2.zero;
				dmgable.LockVelocity = true;
				StartCoroutine(DelayJoint(0.1f));

				return;
			}

			if (touchDirections.IsGrounded)
			{
				rb.gravityScale = gravOriginalScale;
				rb.mass = 1;

				wallSlideTimer = 0;
				dashNo = 1;

				CanWallJump = false;
				canDash = true;

				coyoteTimeCounter = coyoteTime;
			}
			else
			{
				coyoteTimeCounter -= Time.deltaTime;
			}

			if (dashNo <= 0 || !dashEnabled || !canDash)
			{
				material.SetInt(outline, 0);
			} else
			{
				material.SetInt(outline, 1);
			}

			if (touchDirections.IsOnWall && !touchDirections.IsGrounded && !ladderMovement.isClimbingLadder)
			{
				CanWallJump = true;
				canWallSlide = true;
				IsWallGrabing = true;
			}
			else
			{
				IsWallGrabing = false;
				canWallSlide = false;
			}

			if (CanWallJump)
			{
				lastWallJumpDir = IsFacingRight ? -1 : 1;
				WallJump(lastWallJumpDir);
			}

			if (playerInput.actions["Jump"].IsPressed())
			{
				jumpBufferCounter = jumpBufferTime;
			}
			else
			{
				jumpBufferCounter -= Time.deltaTime;
			}

			if (playerInput.actions["Jump"].IsPressed() && IsWallGrabing == true)
			{
				playerInput.actions["Grab"].Disable();
			}
			else
			{
				playerInput.actions["Grab"].Enable();
			}

			if (isWallJumping && moveInput.x != 0)
			{
				SetFacingDirection(moveInput);
			}

			//BetterJump
			if (rb.velocity.y < 0)
			{
				if (!isDashing || !isWallJumping || !IsGrappling)
				{
					rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
				}
			}
			else if (rb.velocity.y > 0 && !playerInput.actions["Jump"].IsPressed())
			{
				if (!isDashing || !isWallJumping || !IsGrappling)
				{
					rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
				}
			}

			if (rb.velocity.y < fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpYDamping && !CameraManager.instance.LerpedFromFalling)
			{
				CameraManager.instance.LerpYDamping(true);
			}
			if (rb.velocity.y > 0f && !CameraManager.instance.IsLerpYDamping && !CameraManager.instance.LerpedFromFalling)
			{
				CameraManager.instance.LerpedFromFalling = false;

				CameraManager.instance.LerpYDamping(false);
			}

			NodeBehaviour();
			gameObject.tag = originalTag;
		}
		else
		{
			gameObject.tag = ("Untagged");
			rb.gravityScale = 0;
			joint.enabled = false;
			DeselectNode();
		}
	}

	private void FixedUpdate()
	{
		if (IsAlive)
		{
			hMove = horizontalMoveAction.ReadValue<Vector2>().x;
			vMove = verticalMoveAction.ReadValue<Vector2>().y;

			if (!dmgable.LockVelocity)
			{
				#region Movement
				if (isOnPlatform)
				{
					float targetSpeed = moveInput.x * walkSpeed;

					targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, 1);

					#region Calculate AccelRate
					float accelRate;

					if (touchDirections.IsGrounded)
						accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;
					else
						accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount * accelInAir : runDeccelAmount * deccelInAir;
					#endregion

					#region Add Bonus Jump Apex Acceleration
					//if (playerInput.actions["Jump"].IsPressed() && Mathf.Abs(rb.velocity.y) < jumpHangTimeThreshold)
					//{
					//	accelRate *= jumpHangAccelerationMult;
					//	targetSpeed *= jumpHangMaxSpeedMult;
					//}
					#endregion

					float speedDif = targetSpeed - rb.velocity.x;

					float movement = speedDif * accelRate;

					rb.AddForce((movement * Vector2.right) + platformRb.velocity, ForceMode2D.Force);
				}
				else if (!isWallJumping)
				{
					Movement();
				}
				#endregion

			}

			if (isOnJumpPad)
			{
				StartCoroutine(DisableJumpUntilLanding());
			}

			#region Wallslide
			if (touchDirections.IsOnWall && !touchDirections.IsGrounded)
			{
				if (canWallSlide)
				{
					wallSlideTimer += Time.deltaTime;

					if (wallSlideTimer >= wallSlideDelay)
					{
						if (playerInput.actions["Grab"].IsPressed())
						{
							rb.gravityScale = gravOriginalScale;
							WallSlide();
						}
						else
						{
							rb.gravityScale = gravOriginalScale;
						}
					}
					else if (playerInput.actions["Grab"].IsPressed())
					{
						WallSlide();
						rb.gravityScale = 0;
						rb.velocity = Vector2.zero;
					}
					else
					{
						//WallSlide();
						rb.gravityScale = gravOriginalScale;
					}
				}
			}
			else if (!isDashing || !playerInput.actions["Grapple"].IsPressed())
			{
				wallSlideTimer = 0;
				rb.gravityScale = gravOriginalScale;
			}
			#endregion
		}

		//if (!touchDirections.IsGrounded && !isDashing && ladderMovement.isClimbingLadder && touchDirections.IsOnWall && playerInput.actions["Grab"].IsPressed())
		//	CheckForLedge();
		animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
	}

	private IEnumerator DisableJumpUntilLanding()
	{
		playerInput.actions["Jump"].Disable();

		yield return new WaitUntil(() => touchDirections.IsGrounded || touchDirections.IsOnWall);

		playerInput.actions["Jump"].Enable();
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		moveInput = context.ReadValue<Vector2>();

		if (IsAlive)
		{
			IsMoving = moveInput != Vector2.zero;

			SetFacingDirection(moveInput);
		}
		else
		{
			IsMoving = false;
		}
	}

	void Movement()
	{
		//Calculate the direction we want to move in and our desired velocity
		float targetSpeed = moveInput.x * walkSpeed;
		//We can reduce are control using Lerp() this smooths changes to are direction and speed
		targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, 1);

		#region Calculate AccelRate
		float accelRate;

		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		if (touchDirections.IsGrounded)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount * accelInAir : runDeccelAmount * deccelInAir;
		#endregion

		#region Add Bonus Jump Apex Acceleration
		////Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
		//if (playerInput.actions["Jump"].IsPressed() || isWallJumping && Mathf.Abs(rb.velocity.y) < jumpHangTimeThreshold)
		//{
		//	accelRate *= jumpHangAccelerationMult;
		//	targetSpeed *= jumpHangMaxSpeedMult;
		//}
		#endregion

		#region Conserve Momentum
		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if (doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && !touchDirections.IsGrounded)
		{
			//Prevent any deceleration from happening, or in other words conserve are current momentum
			accelRate = 0;
		}
		#endregion

		//Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - rb.velocity.x;

		//Calculate force along x-axis to apply to thr player
		float movement = speedDif * accelRate;

		//Convert this to a vector and apply to rigidbody
		rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			jumpBufferCounter = jumpBufferTime;
			if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
			{
				AudioManager.Instance.PlaySFX(AudioManager.Instance.jump);
				animator.SetTrigger(AnimationStrings.jumpTrigger);
				float force = jumpImpulse;
				if (rb.velocity.y < 0)
					force -= rb.velocity.y;

				rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

				coyoteTimeCounter = 0f;
				jumpBufferCounter = 0f;
			}
		} 
	}

	private void WallJump(int dir)
	{
		if (IsWallGrabing)
		{
			isWallJumping = false;
			wallJumpDir = dir;
			wallJumpCounter = wallJumpTime;

			CancelInvoke(nameof(StopWallJump));
		}
		else
		{
			wallJumpCounter = 0;
		}

		if (playerInput.actions["Jump"].triggered && wallJumpCounter > 0f)
		{
			isWallJumping = true;
			AudioManager.Instance.PlaySFX(AudioManager.Instance.jump);

			CancelInvoke(nameof(WallSlide));

			Vector2 force = new Vector2(wallJumpDir * wallJumpPower.x, wallJumpPower.y);

			if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
				force.x -= rb.velocity.x;

			if (rb.velocity.y < 0)
				force.y -= rb.velocity.y;

			rb.AddForce(force, ForceMode2D.Impulse);
			wallJumpCounter = 0f;

			IsFacingRight = (wallJumpDir > 0);

			Invoke(nameof(StopWallJump), wallJumpDuration);
		}

	}

	private void StopWallJump()
	{
		playerInput.actions["Jump"].Enable();
		isWallJumping = false;
		CanWallJump = false;
	}

	//private void CheckForLedge()
	//{
	//	if (touchDirections.ledgeDetected && canGrabLedge)
	//	{
	//		canGrabLedge = false;

	//		Vector2 ledgePosition = GetComponentInChildren<EdgeDetect>().transform.position;

	//		if (IsFacingRight)
	//		{
	//			climbBegunPosition = ledgePosition + offset1;
	//			climbOverPosition = ledgePosition + offset2;
	//		}
	//		else
	//		{
	//			climbBegunPosition = ledgePosition + new Vector2(-offset1.x, offset1.y);
	//			climbOverPosition = ledgePosition + new Vector2(-offset2.x, offset2.y);
	//		}

	//		canClimb = true;

	//		animator.SetBool(AnimationStrings.canClimb, canClimb);
	//	}

	//	if (canClimb)
	//	{
	//		rb.gravityScale = 0;
	//		dmgable.LockVelocity = true;
	//		playerInput.actions["Jump"].Disable();
	//		playerInput.actions["HorizontalMove"].Disable();

	//		transform.position = climbBegunPosition;
	//	}
	//}

	private void AllowLedgeClimb()
	{
		canGrabLedge = true;
	}

	private void ClimbedLedgePosition()
	{
		dmgable.LockVelocity = false;
		playerInput.actions["Jump"].Enable();
		playerInput.actions["HorizontalMove"].Enable();

		rb.gravityScale = gravOriginalScale;

		canClimb = false;
		transform.position = climbOverPosition;
		Invoke(nameof(AllowLedgeClimb), .1f);
		animator.SetBool(AnimationStrings.canClimb, canClimb);
	}

	private void WallSlide()
	{
		if (!CanMove || isWallJumping)
			return;

		if (playerInput.actions["Jump"].triggered)
		{
			return;
		}

		bool pushingWall = false;
		if ((rb.velocity.x > 0 && touchDirections.IsOnWall && canWallSlide) || (rb.velocity.x < 0 && touchDirections.IsOnWall && canWallSlide))
		{
			pushingWall = true;
		}
		float push = pushingWall ? 0 : rb.velocity.x;

		rb.velocity = new Vector2(push, -slideSpeed);
		particleController.PlaySlideParticle();
	}

	public void OnDash(InputAction.CallbackContext context)
	{
		if (context.started && canDash && dashNo > 0 && dashEnabled)
		{
			AudioManager.Instance.PlaySFX(AudioManager.Instance.dash);

			dashNo = 0;
			playerInput.actions["Jump"].Disable();
			playerInput.actions["Dash"].Disable();
			canDash = false;
			isDashing = true;

			animator.SetBool(AnimationStrings.isDashing, isDashing);
			StartCoroutine(StopDash());
		}
	}

	private IEnumerator ApplyDashDelay()
	{
		CameraShakeManager.instance.Shake(impulseSource);

		yield return new WaitForSeconds(dashDelay);
		ghostTrail.makeGhost = true;

		dashDir = new Vector2(hMove, vMove);

		if (dashDir == Vector2.zero)
		{
			if (!IsFacingRight)
			{
				dashDir = Vector2.left;
			}
			else
			{
				dashDir = Vector2.right;
			}
		}

		rb.velocity = dashDir.normalized * dashPower;
	}

	private IEnumerator StopDash()
	{
		dashNo--;

		yield return new WaitForSeconds(dashTime);

		isDashing = false;

		playerInput.actions["Jump"].Enable();
		playerInput.actions["Dash"].Enable();
		rb.gravityScale = gravOriginalScale;
		animator.SetBool(AnimationStrings.isDashing, isDashing);
		Debug.Log(dashNo);
		yield return new WaitForSeconds(0.5f);
		ghostTrail.makeGhost = false;
	}

	public void RefreshDash(float amount)
	{
		if (dashNo + amount > maxDash) { amount = maxDash - dashNo; }
        Debug.Log(dashNo);

        canDash = true;

		dashNo += amount;
	}

	public void OnHit(int dmg, Vector2 knockback)
	{
		playerRespawn.Die();

		StartCoroutine(ApplyHitFx(1.3f));
		CameraShakeManager.instance.Shake(impulseSource);
	}


	private IEnumerator ApplyHitFx(float fxDuration)
	{
		float elapsedTime = 0f;
		while (elapsedTime < fxDuration)
		{
			elapsedTime += Time.deltaTime;

			float lerpedAmount = Mathf.Lerp(0f, 1f, (elapsedTime / fxDuration));
			material.SetFloat(hitEffectBlend, lerpedAmount);

			yield return null;
		}

		elapsedTime = 0f;
		while (elapsedTime < fxDuration)
		{
			elapsedTime += Time.deltaTime;

			float lerpedAmount = Mathf.Lerp(1f, 0f, (elapsedTime / fxDuration));
			material.SetFloat(hitEffectBlend, lerpedAmount);

			yield return null;
		}
	}
	private void SetFacingDirection(Vector2 moveInput)
	{
		if (moveInput.x > 0 && !IsFacingRight)
		{
			IsFacingRight = true;
		}
		else if (moveInput.x < 0 && IsFacingRight)
		{
			IsFacingRight = false;
		}
	}

	public void OnInteract(InputAction.CallbackContext context)
	{
		playerInteract.CheckInteraction();
	}

	public void OnGrapple(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Node closestNode = node.FindClosestNode();

			if (closestNode != null)
			{
				grappleRope.StartRopeAnim(closestNode.transform.position);
				grappleRope.target = closestNode.transform;

				SelectNode(closestNode);
			}
		}
	}

	public void SelectNode(Node node)
	{
		selectedNode = node;
	}

	private IEnumerator GrappleToNode()
	{
		joint.connectedBody = selectedNode.GetComponent<Rigidbody2D>();
		IsGrappling = true;
		Vector2 direction = (selectedNode.transform.position - transform.position).normalized;
		playerInput.actions["Jump"].Disable();

		if (Vector2.Distance(transform.position, selectedNode.transform.position) < 0.5f)
		{			
			selectedNode = null;
			IsGrappling = false;

			AudioManager.Instance.PlaySFX(AudioManager.Instance.grapple);

			rb.velocity = direction * grappleForce;
			Vector2 launchForce = direction * 30f;

			if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(launchForce.x))
				launchForce.x -= rb.velocity.x;

			if (rb.velocity.y < 0)
				launchForce.y -= rb.velocity.y;

			rb.AddForce(launchForce, ForceMode2D.Impulse);

			StartCoroutine(DelayGrappling(0.17f));
			Invoke(nameof(DeselectNode), 0.1f);
		}

		yield return null;
	}

	private IEnumerator DelayJoint(float delay)
	{
		yield return new WaitForSeconds(delay);
		IsGrappling = false;
		joint.distance = 0.005f;
		dmgable.LockVelocity = false;


	}
	private IEnumerator DelayGrappling(float delay)
	{
		playerInput.actions["Grapple"].Disable();
		yield return new WaitForSeconds(delay);
		playerInput.actions["Grapple"].Enable();
	}

	public void DeselectNode()
	{
		IsGrappling = false;
		dmgable.LockVelocity = false;
		selectedNode = null;
		grappleRope.StopRopeAnim();
		playerInput.actions["Jump"].Enable();

		StopCoroutine(GrappleToNode());
	}

	public void NodeBehaviour()
	{
		if (selectedNode == null)
		{
			joint.connectedBody = null;
			line.enabled = false;
			joint.enabled = false;
			
			return;
		}

		line.enabled = true;

		if (selectedNode != null)
		{
			joint.enabled = true;

			StartCoroutine(GrappleToNode());
		}
	}

	public void OnRanged(InputAction.CallbackContext context)
	{
		if (context.started && rangedEnabled && canAttack)
		{
			animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
			StartCoroutine(RangedAttackCooldown());
		}
	}

	private IEnumerator RangedAttackCooldown()
	{
		canAttack = false;
		yield return new WaitForSeconds(rangedCooldown);
		canAttack = true;
	}

	#region Save/Load

	public void Save(ref PlayerSaveData data)
	{
		data.Position = transform.position;
	}

	public void Load(PlayerSaveData data)
	{
		transform.position = data.Position;
	}

	#endregion
}

[System.Serializable]
public struct PlayerSaveData
{
	public Vector3 Position;
}
