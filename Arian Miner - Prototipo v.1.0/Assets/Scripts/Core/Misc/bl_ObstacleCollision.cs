using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class bl_ObstacleCollision : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Player"))
        {
            bl_ObstacleRoot t = GetComponentInParent<bl_ObstacleRoot>();
            t.DoExplosion(other.transform);
            bl_GameManager.Instance.Hit();
            //bl_GameManager.Instance.GameOver();
        }
    }
}