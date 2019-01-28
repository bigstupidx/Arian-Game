////////////////////////////////////////////////////////////////////////////
// bl_RotationDynamic
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System;

[Serializable]
public class bl_RotationDynamic
{

    public bool Enable = false;
    public bool AutoRotate;
    public Vector3 To;
    [Range(0, 20)]
    public float Lerp = 12;
     [Range(0,20)]public float Delay = 12;

    private Transform m_Transform;
    private bool isForward = true;
    private Vector3 defaultPosition;
    private Vector3 CurrentPosition;
    private float rate;

    /// <summary>
    /// 
    /// </summary>
    public void Init(Transform t)
    {
        m_Transform = t;
        defaultPosition = m_Transform.localEulerAngles;
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnUpdate()
    {
        if (!Enable)
            return;

        if (AutoRotate)
        {
            m_Transform.Rotate((To * Lerp) * Time.timeScale);
        }
        else
        {
            if (Delay > 0)
            {
                if (rate > Time.time)
                    return;
            }

            if (isForward)
            {
                CurrentPosition = To;
            }
            else
            {
                CurrentPosition = defaultPosition;
            }

            m_Transform.localRotation = Quaternion.Slerp(m_Transform.localRotation, Quaternion.Euler(CurrentPosition), (Time.deltaTime * Speedbox.bl_Utils.MathfUtils.EaseLerp(Lerp)) * Time.timeScale);
            if (Vector3.Distance(m_Transform.localEulerAngles, CurrentPosition) < 0.2f)
            {
                isForward = !isForward;
                rate = Time.time + Delay;
            }
        }
    }
}