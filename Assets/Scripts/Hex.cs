using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Hex : MonoBehaviour
{
	MeshFilter m_MeshFilter;
	
	public float m_Radius = 1f;

	void OnEnable()
	{
		m_MeshFilter = GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		
		// Top
		vertices.Add(new Vector3(0f, 0f, 0f));
		normals.Add(Vector3.up);
		uvs.Add(Vector2.zero);
		for(int i=0; i<6; ++i)
		{
			float angle = i / 6f * 2f * Mathf.PI;
			vertices.Add(new Vector3(m_Radius * Mathf.Cos(angle), 0f, m_Radius * Mathf.Sin(angle)));
			normals.Add(Vector3.up);
			uvs.Add(new Vector2(m_Radius * Mathf.Cos(angle), m_Radius * Mathf.Sin(angle)));
		}
		List<int> indices = new List<int>();
		for(int i=1; i<6; ++i)
		{
			indices.Add(0);
			indices.Add(i+1);
			indices.Add(i);
		}
		indices.Add(0);
		indices.Add(1);
		indices.Add(6);
		
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.normals = normals.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = indices.ToArray();
		m_MeshFilter.mesh = mesh;
	}
	
	public Vector2 PositionToAxialCoordinates(Vector3 a_Position)
	{
		Vector3 cubeCoordinates = Vector2.zero;
		cubeCoordinates.y = 2f / 3f * a_Position.x / m_Radius;
		cubeCoordinates.x = (1f / 3f * Mathf.Sqrt(3f) * a_Position.z - 1f / 3f * a_Position.x) / m_Radius;
		cubeCoordinates.z = -cubeCoordinates.x - cubeCoordinates.y;
		
		Vector3 roundedCubeCoordinates = Vector3.zero;
		roundedCubeCoordinates.x = Mathf.Round(cubeCoordinates.x);
		roundedCubeCoordinates.y = Mathf.Round(cubeCoordinates.y);
		roundedCubeCoordinates.z = Mathf.Round(cubeCoordinates.z);
		
		Vector3 diff = Vector3.zero;
		diff.x = Mathf.Abs(roundedCubeCoordinates.x - cubeCoordinates.x);
		diff.y = Mathf.Abs(roundedCubeCoordinates.y - cubeCoordinates.y);
		diff.z = Mathf.Abs(roundedCubeCoordinates.z - cubeCoordinates.z);
		
		if ((diff.x > diff.y) && (diff.x > diff.z))
		{
			roundedCubeCoordinates.x = -roundedCubeCoordinates.y-roundedCubeCoordinates.z;	
		}
		else if (diff.y > diff.z)
		{
			roundedCubeCoordinates.y = -roundedCubeCoordinates.x-roundedCubeCoordinates.z;
		}
		else
		{
			roundedCubeCoordinates.z = -roundedCubeCoordinates.x-roundedCubeCoordinates.y;
		}	
		
		return roundedCubeCoordinates;
	}
	
	Vector3 AxialCoordinatesToPosition(Vector2 a_AxialCoordinates)
	{
		return m_Radius * new Vector3(3f/2f * a_AxialCoordinates.y,
			0f,
			Mathf.Sqrt(3f) * (a_AxialCoordinates.x + a_AxialCoordinates.y/2f));
	}
	
	void Update()
	{
		if(Application.isEditor && !Application.isPlaying)
		{
			Transform cachedTransform = transform;
			Vector2 newCoordinates = PositionToAxialCoordinates(cachedTransform.position);
			Vector3 newPosition = AxialCoordinatesToPosition(newCoordinates);
			cachedTransform.position = newPosition;
		}
	}
}
