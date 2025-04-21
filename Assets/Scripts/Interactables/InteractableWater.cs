#if UNITY_EDITOR // => Ignore from here to next endif if not in editor
using UnityEditor;
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UIElements;



[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(EdgeCollider2D))]
[RequireComponent (typeof(WaterTriggerHandler))]
public class InteractableWater : MonoBehaviour
{
	[Header("Mesh Generate")]
	[Range(2, 500)] public int numOfXVerticles = 70;

	public float width = 10f;
	public float height = 4f;
	public Material waterMaterial;
	private const int NUM_OF_Y_VERTICLES = 2;

	[Header("Gizmo")]
	public Color GizmoColor = Color.white;

	private Mesh mesh;
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private Vector3[] verticles;
	private int[] topVerticlesIndex;

	private EdgeCollider2D coll;

	private void Start()
	{
		GeneratingMesh();
	}

	private void Reset()
	{
		coll = GetComponent<EdgeCollider2D>();
		coll.isTrigger = true;
	}

	public void ResetCollider()
	{
		coll = GetComponent<EdgeCollider2D>();

		Vector2[] newPoints = new Vector2[2];

		Vector2 firstPoint = new Vector2(verticles[topVerticlesIndex[0]].x, verticles[topVerticlesIndex[0]].y);
		newPoints[0] = firstPoint;

		Vector2 secondPoint = new Vector2(verticles[topVerticlesIndex[topVerticlesIndex.Length - 1]].x, verticles[topVerticlesIndex[topVerticlesIndex.Length - 1]].y);
		newPoints[1] = secondPoint;

		coll.offset = Vector2.zero;
		coll.points = newPoints;
	}

	public void GeneratingMesh()
	{
		mesh = new Mesh();

		//Add verticles
		verticles = new Vector3[numOfXVerticles * NUM_OF_Y_VERTICLES];
		topVerticlesIndex = new int[numOfXVerticles];

		for (int y = 0; y < NUM_OF_Y_VERTICLES; y++)
		{
			for (int x = 0; x < numOfXVerticles; x++)
			{
				float xPos = (x / (float)(numOfXVerticles - 1) * width - width / 2);
				float yPos = (y / (float)(NUM_OF_Y_VERTICLES - 1) * height - height / 2);
				verticles[y * numOfXVerticles + x] = new Vector3(xPos, yPos, 0);

				if (y == NUM_OF_Y_VERTICLES-1)
				{
					topVerticlesIndex[x] = y * numOfXVerticles + x;
				}
			}
		}

		//Construct triangles
		int[] triangles = new int[(numOfXVerticles - 1) * (NUM_OF_Y_VERTICLES - 1) * 6];
		int index = 0;

		for (int y = 0; y < NUM_OF_Y_VERTICLES - 1; y++)
		{
			for (int x = 0; x < numOfXVerticles - 1; x++)
			{
				int bottomLeft = y * numOfXVerticles + x;
				int bottomRight = bottomLeft + 1;
				int topLeft = bottomLeft + numOfXVerticles;
				int topRight = topLeft + 1;

				//1st triangle
				triangles[index++] = bottomLeft;
				triangles[index++] = topLeft;
				triangles[index++] = bottomRight;

				//2nd triangle
				triangles[index++] = bottomRight;
				triangles[index++] = topLeft;
				triangles[index++] = topRight;
			}
		}

		//UVs
		Vector2[] uvs = new Vector2[verticles.Length];
		for (int i = 0; i < verticles.Length; i++)
		{
			uvs[i] = new Vector2((verticles[i].x + width / 2) / width, (verticles[i].y + height / 2) / height);
		}

		if (meshRenderer == null)
		{
			meshRenderer = GetComponent<MeshRenderer>();
		}

		if (meshFilter == null)
		{
			meshFilter = GetComponent<MeshFilter>();
		}

		meshRenderer.material = waterMaterial;

		mesh.vertices = verticles;
		mesh.triangles = triangles;
		mesh.uv = uvs;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		meshFilter.mesh = mesh;
	}
}
#if UNITY_EDITOR
[CustomEditor(typeof(InteractableWater))]
public class InteractableWaterEditor : Editor
{
	private InteractableWater water;

	private void OnEnable()
	{
		water = (InteractableWater)target;
	}

	public override VisualElement CreateInspectorGUI()
	{
		VisualElement root = new VisualElement();

		InspectorElement.FillDefaultInspector(root, serializedObject, this);

		root.Add(new VisualElement { style = { height = 10 } });

		Button generateMeshButton = new Button(() => water.GeneratingMesh())
		{
			text = "Generate Mesh"
		};
		root.Add(generateMeshButton);

		Button placeEdgeCollButton = new Button(() => water.ResetCollider())
		{
			text = "Place Edge Collider"
		};
		root.Add(placeEdgeCollButton);

		return root;
	}

	private void ChangeDimension(ref float width, ref float height, float calculatedWidthMax, float calculatedHeightMax)
	{
		width = Mathf.Max(0.1f, calculatedWidthMax);
		height = Mathf.Max(0.1f, calculatedHeightMax);
	}

	private void OnSceneGUI()
	{
		//Draw Wireframe box
		Handles.color = water.GizmoColor;
		Vector3 center = water.transform.position;
		Vector3 size = new Vector3(water.width, water.height, 0.1f);
		Handles.DrawWireCube(center, size);

		//Handles height & width
		float handleSize = HandleUtility.GetHandleSize(center) * 0.1f;
		Vector3 snap = Vector3.one * 0.1f;

		//Corner handles
		Vector3[] corners = new Vector3[4];
		corners[0] = center + new Vector3(-water.width / 2, -water.height / 2, 0); //Bottom left
		corners[1] = center + new Vector3(water.width / 2, -water.height / 2, 0); //Bottom right
		corners[2] = center + new Vector3(-water.width / 2, water.height / 2, 0); //Top left
		corners[3] = center + new Vector3(water.width / 2, water.height / 2, 0); //Top right

		//Handles each corner
		EditorGUI.BeginChangeCheck();
		Vector3 newBottomLeft = Handles.FreeMoveHandle(corners[0], handleSize, snap, Handles.CubeHandleCap);
		if (EditorGUI.EndChangeCheck())
		{
			ChangeDimension(ref water.width, ref water.height, corners[1].x - newBottomLeft.x, corners[3].y - newBottomLeft.y);
			water.transform.position += new Vector3((newBottomLeft.x - corners[0].x) / 2, (newBottomLeft.y - corners[0].y) / 2, 0);
		}

		EditorGUI.BeginChangeCheck();
		Vector3 newBottomRight = Handles.FreeMoveHandle(corners[1], handleSize, snap, Handles.CubeHandleCap);
		if (EditorGUI.EndChangeCheck())
		{
			ChangeDimension(ref water.width, ref water.height, newBottomRight.x - corners[0].x, corners[3].y - newBottomRight.y);
			water.transform.position += new Vector3((newBottomRight.x - corners[1].x) / 2, (newBottomRight.y - corners[1].y) / 2, 0);
		}

		EditorGUI.BeginChangeCheck();
		Vector3 newTopLeft = Handles.FreeMoveHandle(corners[2], handleSize, snap, Handles.CubeHandleCap);
		if (EditorGUI.EndChangeCheck())
		{
			ChangeDimension(ref water.width, ref water.height, corners[3].x - newTopLeft.x, newTopLeft.y - corners[0].y);
			water.transform.position += new Vector3((newTopLeft.x - corners[2].x) / 2, (newTopLeft.y - corners[2].y) / 2, 0);
		}

		EditorGUI.BeginChangeCheck();
		Vector3 newTopRight = Handles.FreeMoveHandle(corners[3], handleSize, snap, Handles.CubeHandleCap);
		if (EditorGUI.EndChangeCheck())
		{
			ChangeDimension(ref water.width, ref water.height, newTopRight.x - corners[2].x, newTopRight.y - corners[1].y);
			water.transform.position += new Vector3((newTopRight.x - corners[3].x) / 2, (newTopRight.y - corners[3].y) / 2, 0);
		}

		if (GUI.changed)
		{
			water.GeneratingMesh();
		}
	}
}
#endif
