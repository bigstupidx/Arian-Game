using UnityEngine;
using System.Collections;

public class bl_SideLimitDetector : MonoBehaviour {

    [Header("Sides")]
    public LevelSides RightChange;
    public LevelSides LeftChange;

    [Header("Gizmo")]
    public Color m_GizmoColor;
    public bool DrawGizmo = false;

    private bl_ChangerManager CM;

    void Awake()
    {
        CM = bl_ChangerManager.Instance;
    }

    void OnTriggerStay(Collider c)
    {
        if(c.tag == "Player")
        {
            if (CM.Side == LeftChange)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (CM.Side == RightChange)
                        return;

                    CM.Change(RightChange,false);
                }
            }

            if (CM.Side == RightChange)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (CM.Side == LeftChange)
                        return;

                    CM.Change(LeftChange, false);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!DrawGizmo)
            return;

        Collider c = GetComponent<Collider>();
        Gizmos.color = m_GizmoColor;
        Gizmos.DrawCube(c.bounds.center, c.bounds.size);
    }
}