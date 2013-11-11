using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	Vector2 m_CurrentPosition = Vector2.zero;
	public GameObject m_MarkerPrefab;
	GameObject m_Marker;
	
	public Dictionary<Vector2, Hexagon> m_Map = new Dictionary<Vector2, Hexagon>();
	
	List<int> m_NextHexagonIds = new List<int>();
	public List<Hexagon> m_NextHexagons = new List<Hexagon>();
	
	public Hexagon m_HexagonPrefab;
	
	public List<Material> m_Materials = new List<Material>();
	public Material m_BackgroundMaterial;
	
	public GUIText m_ScoreText;
	int m_Score;
	
	public Transform m_MapTransform;
	void Start ()
	{
		Application.targetFrameRate = 300;
		
		// Marker
		m_CurrentPosition = Vector2.zero;
		m_Marker = GameObject.Instantiate(m_MarkerPrefab) as GameObject;
		m_Marker.transform.position = _AxialCoordinatesTo3DCoordinates(m_CurrentPosition) + Vector3.up;
		
		// Setup the map
		Hex[] hexes = m_MapTransform.GetComponentsInChildren<Hex>();
		foreach(Hex h in hexes)
		{
			Vector2 axialCoordinates = h.PositionToAxialCoordinates(h.transform.position);
			h.gameObject.SetActive(false);
			if (m_Map.ContainsKey(axialCoordinates)) continue;
			
			Vector3 position = _AxialCoordinatesTo3DCoordinates(axialCoordinates);
			Hexagon hex = GameObject.Instantiate(m_HexagonPrefab, position, Quaternion.identity) as Hexagon;
			hex.m_Coordinates = axialCoordinates;
			hex.m_GameManager = this;
			hex.GetComponent<Renderer>().material = m_BackgroundMaterial;
			hex.m_Id = -1;
			hex.name = "Hexagon (" + axialCoordinates.x + ", " + axialCoordinates.y + ")";
			m_Map[axialCoordinates] = hex;
		}
		
		m_NextHexagonIds.Add(Random.Range(0, m_Materials.Count));
		m_NextHexagonIds.Add(Random.Range(0, m_Materials.Count));
		m_NextHexagonIds.Add(Random.Range(0, m_Materials.Count));
		FixNextHexagonMaterials();
		
		CreateHexagon(Vector2.zero);
		
		m_ScoreText.text = m_Score.ToString();
	}

	Hexagon CreateHexagon(Vector2 a_AxialCoordinates)
	{
		int index = m_NextHexagonIds[0];
		
		Hexagon hex = m_Map[a_AxialCoordinates];
		hex.GetComponent<Renderer>().material = m_Materials[index];
		hex.m_Id = index;
		
		m_NextHexagonIds[0] = m_NextHexagonIds[1]; 
		m_NextHexagonIds[1] = m_NextHexagonIds[2]; 
		m_NextHexagonIds[2] = Random.Range(0, m_Materials.Count);
		FixNextHexagonMaterials();
		
		return hex;
	}

	void DestroyHexagon(Hexagon a_Hexagon)
	{
		a_Hexagon.GetComponent<Renderer>().material = m_BackgroundMaterial;
		a_Hexagon.m_Id = -1;
	}
	
	Vector2 m_BeginningMousePosition = Vector2.zero;
	bool m_IsSwipeActive = false;
	void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			m_BeginningMousePosition = mousePosition;
			m_IsSwipeActive = true;
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (m_IsSwipeActive)
			{
				m_IsSwipeActive = false;
				Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				Vector2 delta = mousePosition - m_BeginningMousePosition;
				if (delta.magnitude > 80f)
				{
					m_IsSwipeActive = false;
					SwipeTo(delta);
				}
			}
		}
		if (Input.GetMouseButton(0))
		{
			if (m_IsSwipeActive)
			{
				Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				Vector2 delta = mousePosition - m_BeginningMousePosition;
				if (delta.magnitude > 80f)
				{
					m_IsSwipeActive = false;
					SwipeTo(delta);
				}
			}
		}
	}
	
	List<Hexagon> m_Combos = new List<Hexagon>();
	void SwipeTo(Vector2 a_Delta)
	{
		Vector2 newPosition = m_CurrentPosition + _InputDeltaToDisplacement(a_Delta);		
		
		if (!m_Map.ContainsKey(newPosition)) return;
		if (m_Map[newPosition].m_Id != -1) return;
		
		m_Score += 1;
		
		m_CurrentPosition = newPosition;	
		m_Marker.transform.position = _AxialCoordinatesTo3DCoordinates(m_CurrentPosition) + Vector3.up;
		
		Hexagon hex = CreateHexagon(m_CurrentPosition);
		
		// Check for combos
		m_Combos.Clear();
		RecursivelyGetSameColorHexagons(hex, ref m_Combos);
		if (m_Combos.Count > 2)
		{
			m_Score += 10 * (int)(Mathf.Pow(2, (m_Combos.Count-2)));
			foreach(Hexagon comboHexagon in m_Combos)
			{
				if (comboHexagon != hex)
				{
					DestroyHexagon(comboHexagon);
				}
			}
		}
	
		// Check if there is still some possible moves
		bool validMove = false;
		foreach(Hexagon neighbor in hex.GetNeighbors())
		{
			if(neighbor.m_Id == -1)
			{
				validMove = true;
				break;
			}
		}
		if (!validMove)
		{
			Application.LoadLevel(Application.loadedLevelName);
		}
		
		m_ScoreText.text = m_Score.ToString();
	}
	
	[HideInInspector]
	public Vector2 m_AxialAxis0 = new Vector2(1, 0);
	[HideInInspector]
	public Vector2 m_AxialAxis1 = new Vector2(1, -1);
	[HideInInspector]
	public Vector2 m_AxialAxis2 = new Vector2(0, 1);
	
	Vector2 _InputDeltaToDisplacement(Vector2 a_InputDelta)
	{
		Vector2 axis0 = new Vector2(0f, 1f);
		float dot0 = Vector2.Dot(axis0, a_InputDelta);
		Vector2 axis1 = new Vector2(Mathf.Cos(Mathf.PI / 6f), Mathf.Sin(Mathf.PI / 6f));
		float dot1 = Vector2.Dot(axis1, a_InputDelta);
		Vector2 axis2 = new Vector2(Mathf.Cos(5f * Mathf.PI / 6f), Mathf.Sin(5f * Mathf.PI / 6f));
		float dot2 = Vector2.Dot(axis2, a_InputDelta);
		

		if (Mathf.Abs(dot0) > Mathf.Abs(dot1))
		{
			if (Mathf.Abs(dot0) > Mathf.Abs(dot2))
			{
				return (dot0 > 0f) ? m_AxialAxis0 : -m_AxialAxis0;
			}
			else
			{
				return (dot2 > 0f) ? m_AxialAxis1 : -m_AxialAxis1;				
			}
		}
		else
		{
			if (Mathf.Abs(dot1) > Mathf.Abs(dot2))
			{
				return (dot1 > 0f) ? m_AxialAxis2 : -m_AxialAxis2;	
			}
			else
			{
				return (dot2 > 0f) ? m_AxialAxis1 : -m_AxialAxis1;				
			}	
		}
	}
	
	Vector3 _AxialCoordinatesTo3DCoordinates(Vector2 a_AxialCoordinates)
	{
		float size = 1;
		return size * new Vector3(3f/2f * a_AxialCoordinates.y, 0f, Mathf.Sqrt(3) * (a_AxialCoordinates.x + a_AxialCoordinates.y/2f));
	}
	
	void RecursivelyGetSameColorHexagons(Hexagon a_Hexagon, ref List<Hexagon> a_List)
	{
		foreach(Hexagon neighbor in a_Hexagon.GetNeighbors())
		{
			if ((neighbor.m_Id == a_Hexagon.m_Id) && (!a_List.Contains(neighbor)))
			{
				a_List.Add(neighbor);
				RecursivelyGetSameColorHexagons(neighbor, ref a_List);
			}
		}
	}
	
	void FixNextHexagonMaterials()
	{
		for(int i = 0; i < m_NextHexagons.Count; ++i)
		{
			Material m = m_Materials[m_NextHexagonIds[i]];
			m_NextHexagons[i].GetComponent<Renderer>().material = m;
		}
	}
	
	public float GetDistanceBetween(Vector2 a_Coordinates1, Vector2 a_Coordinates2)
	{
	    return (Mathf.Abs(a_Coordinates1.x - a_Coordinates2.x) +
			+ Mathf.Abs(a_Coordinates1.y - a_Coordinates2.y)
			+ Mathf.Abs(a_Coordinates1.x + a_Coordinates1.y - a_Coordinates2.x - a_Coordinates2.y)) / 2f;
	}
	
	public IEnumerable GetNeighborCoordinates()
	{
		yield return -m_AxialAxis0;
		yield return m_AxialAxis0;
		yield return -m_AxialAxis1;
		yield return m_AxialAxis1;
		yield return -m_AxialAxis2;
		yield return m_AxialAxis2;
	}
	
	Vector2 PositionToAxialCoordinates(Vector3 a_Position)
	{
		Vector2 axialCoordinates = Vector2.zero;
		float temp = Mathf.Floor(a_Position.x + Mathf.Sqrt(3f) * a_Position.z + 1f);
		axialCoordinates.x = Mathf.Floor((Mathf.Floor(2f * a_Position.x + 1f) + temp) / 3f);
		axialCoordinates.y = Mathf.Floor((temp + Mathf.Floor(-a_Position.x + Mathf.Sqrt(3f) * a_Position.z + 1f))/3f);
		return axialCoordinates;
	}
}
