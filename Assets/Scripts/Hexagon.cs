using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hexagon : MonoBehaviour
{
	MeshFilter m_MeshFilter;
	
	public float m_Height = 1f;
	public float m_Radius = 1f;
	
	public GameManager m_GameManager;
	
	public int m_Id;

	void Start ()
	{
		m_MeshFilter = GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		
		// Top
		vertices.Add(new Vector3(0f, 0.5f * m_Height, 0f));
		normals.Add(Vector3.up);
		uvs.Add(Vector2.zero);
		for(int i=0; i<6; ++i)
		{
			float angle = i / 6f * 2f * Mathf.PI;
			vertices.Add(new Vector3(m_Radius * Mathf.Cos(angle), 0.5f * m_Height, m_Radius * Mathf.Sin(angle)));
			normals.Add(Vector3.up);
			uvs.Add(new Vector2(m_Radius * Mathf.Cos(angle), m_Radius * Mathf.Sin(angle)));
		}
		// Bottom
		vertices.Add(new Vector3(0f, -0.5f * m_Height, 0f));
		normals.Add(Vector3.down);
		uvs.Add(Vector2.zero);
		for(int i=0; i<6; ++i)
		{
			float angle = i / 6f * 2f * Mathf.PI;
			vertices.Add(new Vector3(m_Radius * Mathf.Cos(angle), -0.5f * m_Height, m_Radius * Mathf.Sin(angle)));
			normals.Add (Vector3.down);
			uvs.Add(new Vector2(m_Radius * Mathf.Cos(angle), m_Radius * Mathf.Sin(angle)));
		}
		// Size
		for(int i=0; i<6; ++i)
		{
			float angle = i / 6f * 2f * Mathf.PI;
			vertices.Add(new Vector3(m_Radius * Mathf.Cos(angle), 0.5f * m_Height, m_Radius * Mathf.Sin(angle)));
			normals.Add(new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)));
			uvs.Add(new Vector2(m_Radius * Mathf.Cos(angle), m_Radius * Mathf.Sin(angle)));
		}
		for(int i=0; i<6; ++i)
		{
			float angle = i / 6f * 2f * Mathf.PI;
			vertices.Add(new Vector3(m_Radius * Mathf.Cos(angle), -0.5f * m_Height, m_Radius * Mathf.Sin(angle)));
			normals.Add(new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)));
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

		for(int i=8; i<13; ++i)
		{
			indices.Add(7);
			indices.Add(i);
			indices.Add(i+1);
		}
		indices.Add(7);
		indices.Add(13);
		indices.Add(8);
		
		for(int i=14; i<19; ++i)
		{
			indices.Add(i);
			indices.Add(i+1);
			indices.Add(i+6);
		}
		indices.Add(19);
		indices.Add(14);
		indices.Add(25);
		for(int i=14; i<19; ++i)
		{
			indices.Add(i+1);
			indices.Add(i+7);
			indices.Add(i+6);
		}
		indices.Add(14);
		indices.Add(20);
		indices.Add(25);
		
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.normals = normals.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = indices.ToArray();
		m_MeshFilter.mesh = mesh;
	}
	
	
	public Vector2 m_Coordinates = Vector2.zero;
	
	public IEnumerable GetNeighbors()
	{
		var map = m_GameManager.m_Map;
		if (map.ContainsKey(m_Coordinates-m_GameManager.m_AxialAxis0))
		{
			yield return map[m_Coordinates-m_GameManager.m_AxialAxis0];
		}
		if (map.ContainsKey(m_Coordinates+m_GameManager.m_AxialAxis0))
		{
			yield return map[m_Coordinates+m_GameManager.m_AxialAxis0];
		}
		if (map.ContainsKey(m_Coordinates-m_GameManager.m_AxialAxis1))
		{
			yield return map[m_Coordinates-m_GameManager.m_AxialAxis1];
		}
		if (map.ContainsKey(m_Coordinates+m_GameManager.m_AxialAxis1))
		{
			yield return map[m_Coordinates+m_GameManager.m_AxialAxis1];
		}
		if (map.ContainsKey(m_Coordinates-m_GameManager.m_AxialAxis2))
		{
			yield return map[m_Coordinates-m_GameManager.m_AxialAxis2];
		}
		if (map.ContainsKey(m_Coordinates+m_GameManager.m_AxialAxis2))
		{
			yield return map[m_Coordinates+m_GameManager.m_AxialAxis2];
		}
	}
}
