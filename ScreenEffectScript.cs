using UnityEngine;
using System.Collections;

public class ScreenEffectScript : MonoBehaviour {

    public float m_EffectPriority = 1.0f; // Range [1-infinity]
    public Material m_Effect;
	public bool m_Animate = false;
    private Camera m_Camera = null;
	private GameObject m_GameObject;
    private GameObject m_EffectRenderer;
    private Mesh m_Plane;
    private bool m_IsInit = false;
	private Animation m_EnableAnimation;
	
	void Start () 
    {

        m_GameObject = gameObject;
        m_Camera = m_GameObject.AddComponent<Camera>();
        m_Camera.orthographic = true;
        m_Camera.orthographicSize = 1.0f;
        m_Camera.clearFlags = CameraClearFlags.Nothing;
        m_Camera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        m_Camera.cullingMask = (1 << 30);
        m_EffectRenderer = new GameObject("EffectRenderer");
        m_EffectRenderer.layer = 30;
        m_EffectRenderer.transform.parent = transform;
        m_EffectRenderer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.5f);
        m_Plane = m_EffectRenderer.AddComponent<MeshFilter>().mesh;
        m_EffectRenderer.AddComponent<MeshRenderer>();
        m_EffectRenderer.renderer.material = m_Effect;
        Vector3[] _Vertices = { new Vector3(-1, -1, 0), new Vector3(1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, 1, 0)};
        Vector2[] _UV = { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) }; ;
        int[] _Indices = {0,2,1,1,2,3};
        m_Plane.vertices = _Vertices;
        m_Plane.uv = _UV;
        m_Plane.triangles = _Indices;
        m_IsInit = true;
	}

    public void Enable()
    {
        if(!m_IsInit)
            return;

        m_Camera.enabled = true;
    }

    public void Disable()
    {
        if (!m_IsInit)
            return;

        m_Camera.enabled = false;
    }

	void Update () 
    {
        if(!m_IsInit)
            return;

        m_EffectRenderer.transform.localScale = new Vector3(m_Camera.pixelWidth / m_Camera.pixelHeight, 1, 1);
	}
}
