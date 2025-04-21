using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node : MonoBehaviour
{
	private GameObject player;
	private PlayerController pController;

	public float maxGrappleDistance = 2f;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		pController = player.GetComponent<PlayerController>();

	}

	public Node FindClosestNode()
	{
		Node[] allNodes = FindObjectsOfType<Node>();
		Node closestNode = null;
		float closestDistance = maxGrappleDistance;

		foreach (Node node in allNodes)
		{
			float distance = Vector2.Distance(player.transform.position, node.transform.position);
			if (distance <= closestDistance)
			{
				closestDistance = distance;
				closestNode = node;
			}
		}

		return closestNode;
	}

	private void SelectNode(Node node)
	{
		Debug.Log("Node Selected");
		pController.SelectNode(node);
	}

	private void DeselectNode()
	{
		Debug.Log("Node Deselected");
		pController.DeselectNode();
	}
}

