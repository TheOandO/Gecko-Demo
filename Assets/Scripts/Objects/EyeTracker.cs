using UnityEngine;

public class EyeFollow : MonoBehaviour
{
	public GameObject trackedObj;

	void Update()
	{
		eyeFollow();
	}

	void eyeFollow()
	{
		Vector3 objPos = trackedObj.transform.position;

		Vector2 direction = new Vector2(
			(objPos.x - transform.position.x),
			(objPos.y - transform.position.y)
		);
		transform.up = direction;
	}
}