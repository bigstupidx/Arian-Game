////////////////////////////////////////////////////////////////////////////
// bl_AutoRotate
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class bl_AutoRotate : MonoBehaviour
{
    [Range(1, 100)]
    public float Speed = 10;

    private Transform m_Transform;
    private Transform Transform
    {
        get
        {
            if(m_Transform == null)
            {
                m_Transform = GetComponent<Transform>();
            }
            return m_Transform;
        }
    }

    void OnEnable()
    {
        StartCoroutine(OnUpdate());
    }
   
    IEnumerator OnUpdate()
    {
        while (true)
        {
            Transform.Rotate(Vector3.forward * Time.deltaTime * Speed, Space.World);
            yield return new WaitForEndOfFrame();
        }
    }
}