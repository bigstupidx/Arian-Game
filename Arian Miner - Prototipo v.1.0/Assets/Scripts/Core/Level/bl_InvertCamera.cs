using UnityEngine;

[RequireComponent(typeof(Camera))]
public class bl_InvertCamera : MonoBehaviour {

    private Camera m_Camera;

    void Awake()
    {
        m_Camera = GetComponent<Camera>();
    }

    void OnPreCull()
    {
        m_Camera.ResetWorldToCameraMatrix();
        m_Camera.ResetProjectionMatrix();
        m_Camera.projectionMatrix = m_Camera.projectionMatrix * Matrix4x4.Scale(new Vector3(1, -1, 1));
    }

    void OnPreRender()
    {
        GL.invertCulling = true;
    }

    void OnPostRender()
    {
        GL.invertCulling = false;
    }
}