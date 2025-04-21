using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
	public float speed;
	public float waitTime = 2f; // Time to wait at each position

	private Vector3 targetPos;

	PlayerController playerController;
	Rigidbody2D rb;
	Vector3 moveDirection;

	public GameObject ways;
	public Transform[] wayPoints;

	int pointIndex;
	int pointCount;
	int direction = 1;

	private Vector3 initialPos;
	private int initialPointIndex;
	private int initialDirection;

	private void Awake()
	{
		playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		rb = GetComponent<Rigidbody2D>();

		wayPoints = new Transform[ways.transform.childCount];
		for (int i = 0; i < ways.gameObject.transform.childCount; i++)
		{
			wayPoints[i] = ways.transform.GetChild(i).gameObject.transform;
		}
	}

	void Start()
	{
		pointIndex = 1;
		pointCount = wayPoints.Length;
		targetPos = wayPoints[1].transform.position; // Start by moving to pos 2
		DirectionCalculate();

		initialPointIndex = pointIndex;
		initialPos = transform.position;
		initialDirection = direction;
	}

	void Update()
	{
		if (Vector2.Distance(transform.position, targetPos) < 0.05f)
		{
			WaitAndMove();
		}
	}
	private void FixedUpdate()
	{
		rb.velocity = moveDirection * speed;
	}

	void WaitAndMove()
	{
		transform.position = targetPos;
		moveDirection = Vector3.zero;

		// Switch the target position
		if (pointIndex == pointCount - 1)
		{
			direction = -1;
		}

		if (pointIndex == 0)
		{
			direction = 1;
		}

		pointIndex += direction;
		targetPos = wayPoints[pointIndex].transform.position;

		StartCoroutine(PFDelay());

	}

	IEnumerator PFDelay()
	{
		yield return new WaitForSeconds(waitTime); // Wait for the specified duration
		DirectionCalculate();
	}

	void DirectionCalculate()
	{
		moveDirection = (targetPos - transform.position).normalized;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerController.isOnPlatform = true;
			playerController.platformRb = rb;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerController.isOnPlatform = false;
		}
	}

	public void ResetPlatform()
	{
		StopAllCoroutines();

		transform.position = initialPos;

		pointIndex = initialPointIndex;
		direction = initialDirection;

		targetPos = wayPoints[pointIndex].transform.position;

		DirectionCalculate();
	}
}
