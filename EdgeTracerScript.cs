using UnityEngine;
using System.Collections;
using System.Collections.Generic;





public class EdgeTracerScript : MonoBehaviour {
	
	public Mesh m; // Mesh to edgeTracer
	public Material m_material; // LineRenderer Material
    public bool m_IsMoving = false; // If gameObject is mobile
    public bool m_AutoRebuild = false;
    public Vector3 m_Offset = new Vector3();
    public Vector3 m_Center = new Vector3();
    public Vector3 m_Scale = new Vector3(1,1,1);
    public bool m_AutoPlacementWithNormal = false;
    public float m_AutoScaleFactor = 0.2f;
    public float m_Width = 0.2f;
    List<Edge> m_edges;
    List<Face> m_faces;
    List<Edge> m_edges_rendered;
    List<LineRenderer> m_gameObjects;
    private Vector3 m_LastPosition; // Last Position known of gameobject

	class Face
	{
		Vector3[] vertex = new Vector3[3];
		
		public Face()
		{
			for(int i = 0; i < 3; i++)
				vertex[i] = new Vector3();
		}
		
		public Face(Vector3 v1, Vector3 v2, Vector3 v3)
		{
			vertex[0] = new Vector3(v1.x, v1.y, v1.z);
			vertex[1] = new Vector3(v2.x, v2.y, v2.z);
			vertex[2] = new Vector3(v3.x, v3.y, v3.z);
		}
		
		public Vector3 normal()
		{
			Vector3 v1 = (vertex[1] - vertex[0]).normalized;
			Vector3 v2 = (vertex[2] - vertex[0]).normalized;
			return Vector3.Normalize(Vector3.Cross(v1,v2));
		}

        public Edge[] getEdges()
        {
            Edge[] _edges = new Edge[3];
            _edges[0] = new Edge(vertex[0], vertex[1]);
            _edges[0].AddFace(this);
            _edges[1] = new Edge(vertex[0], vertex[2]);
            _edges[1].AddFace(this);
            _edges[2] = new Edge(vertex[1], vertex[2]);
            _edges[2].AddFace(this);
            return _edges;
        }
	}
	
	
	class Edge
	{
		public Vector3 start;
		public Vector3 end;

        public static bool Compare(Edge a, Edge b)
        {
            if(a.start.Equals(b.start) && a.end.Equals(b.end) || a.start.Equals(b.end) && a.end.Equals(b.start))
                return true; //Same Edge
            return false;
        }

		public Face[] linkedFace = new Face[2];

        public Edge()
        { }

        public Edge(Vector3 start, Vector3 end)
        {
            this.start = start;
            this.end = end;
        }

		public void AddFace(Face f)
		{
            if (linkedFace[0] == null)
            {
                linkedFace[0] = f;
                return;
            }

			if(linkedFace[1] == null)
				linkedFace[1] = f;
		}
	}
	
	
	// Use this for initialization
	void Start () {

        if (m == null)
            m = transform.gameObject.GetComponent<MeshFilter>().mesh;
		
		m_edges = new List<Edge>();
        m_edges_rendered = new List<Edge>();
        m_faces = new List<Face>();
        m_gameObjects = new List<LineRenderer>();
        GenerateFaces();
        GenerateEdges();
        GenerateLine();
	}

    private void GenerateFaces()
    {
        int max = m.triangles.Length/3;
        for (int i = 0; i < max; i++)
        {
            Face f = new Face(m.vertices[m.triangles[i * 3]], m.vertices[m.triangles[i * 3+1]], m.vertices[m.triangles[i * 3+2]]);
            m_faces.Add(f);
        }

    }

    private void GenerateEdges()
	{
        foreach (Face f in m_faces)
        {
            Edge[] edges = f.getEdges();
            foreach(Edge e in edges)
                AppendEdge(e);
        }
	}
	
	private void AppendEdge(Edge e)
	{
        Edge s = m_edges.Find(
            delegate(Edge a)
            {
                return Edge.Compare(a,e);
            });
        if (s != null)
        {
            s.AddFace(e.linkedFace[0]);
        }
        else
		    m_edges.Add(e);
	}

    Vector3 CalcAutoPlacement(Edge e)
    {
        if (!m_AutoPlacementWithNormal)
            return new Vector3();
        if (e.linkedFace[1] == null)
            return e.linkedFace[0].normal()*m_AutoScaleFactor;

        Vector3 normal = e.linkedFace[0].normal() + e.linkedFace[1].normal();
        return normal.normalized*m_AutoScaleFactor;
    }

    private Vector3 CalcPosition(Vector3 v)
    {
        return transform.position + transform.rotation * (new Vector3(v.x * transform.localScale.x * m_Scale.x, v.y * transform.localScale.y * m_Scale.y, v.z * transform.localScale.z * m_Scale.z));
    }


    private void GenerateLine()
    {
        for(int i = 0; i < m_edges.Count; i++)
        {
            if (m_edges[i].linkedFace[1] != null)
            {
                float a = Mathf.Abs(Vector3.Dot(m_edges[i].linkedFace[0].normal(), m_edges[i].linkedFace[1].normal()));
                if (a > 0.2f)
                    continue;
            }

            GameObject edgeRenderer = new GameObject("EdgeRenderer"+i);
            LineRenderer lineRenderer = edgeRenderer.AddComponent<LineRenderer>();
            lineRenderer.SetColors(Color.black,Color.black);

            lineRenderer.SetPosition(0, CalcPosition(m_edges[i].start + m_Offset + CalcAutoPlacement(m_edges[i])));
            lineRenderer.SetPosition(1, CalcPosition(m_edges[i].end + m_Offset + CalcAutoPlacement(m_edges[i])));
            lineRenderer.SetWidth(m_Width, m_Width);
            edgeRenderer.transform.parent = transform;
            edgeRenderer.transform.localPosition = new Vector3();
            edgeRenderer.transform.localRotation = new Quaternion();
            lineRenderer.material = m_material;
            m_edges_rendered.Add(m_edges[i]);
            m_gameObjects.Add(lineRenderer);
        }
    }

    void UpdatePosition()
    {
        for (int i = 0; i < m_gameObjects.Count; i++)
        {
            m_gameObjects[i].SetPosition(0, CalcPosition(m_edges_rendered[i].start + m_Offset + CalcAutoPlacement(m_edges_rendered[i])));
            m_gameObjects[i].SetPosition(1, CalcPosition(m_edges_rendered[i].end + m_Offset + CalcAutoPlacement(m_edges_rendered[i])));
        }
    }

    void DeleteEdge()
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);
            if (t.name.StartsWith("EdgeRenderer"))
            {
                GameObject.DestroyImmediate(t.gameObject);
                i--;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_AutoRebuild || m_IsMoving && !transform.position.Equals(m_LastPosition))
            UpdatePosition();

        m_LastPosition = transform.position;
	}

}
