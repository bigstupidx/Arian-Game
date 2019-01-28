using UnityEngine;

public class bl_ChangeDetector : MonoBehaviour {

    public LevelSides Side = LevelSides.Up;
    public bool DrawGizmo = false;

	void OnTriggerEnter(Collider c)
    {
        if(c.transform.tag == "Player")
        {
            bl_ChangerManager.Instance.Change(this);
        }
    }

    void OnDrawGizmos()
    {
        if (!DrawGizmo)
            return;

        Collider c = GetComponent<Collider>();
        Gizmos.color = new Color(0, 0.25f, 0.8f,0.7f);
        Gizmos.DrawCube(c.bounds.center, c.bounds.size);
    }
}