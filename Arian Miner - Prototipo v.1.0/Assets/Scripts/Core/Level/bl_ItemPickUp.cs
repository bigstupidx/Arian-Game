using UnityEngine;
using System.Collections;

public class bl_ItemPickUp : MonoBehaviour
{

    void OnTriggerEnter(Collider c)
    {
        if (c.name.Contains("Player"))
        {
            bl_GameManager.Instance.point++;
            transform.parent.parent.GetComponent<bl_ObstacleRoot>().OnPickUp(transform);
            if (bl_ObstaclesManager.Instance.UsePoolMethod)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

}