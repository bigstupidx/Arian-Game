using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bl_ObstacleDynamic : MonoBehaviour
{
    public List<bl_DynamicInfo> Objects = new List<bl_DynamicInfo>();

    [Header("Helper")]
    [SerializeField]private int IndexToTake;

    void OnEnable()
    {
       foreach(bl_DynamicInfo info in Objects)
        {
            if (info.Transform != null)
            {
                info.Position.Init(info.Transform);
                info.Rotation.Init(info.Transform);
                info.Scale.Init(info.Transform);
            }
        }

        StartCoroutine(OnUpdate());
    }

    IEnumerator OnUpdate()
    {
        while (true)
        {
            foreach (bl_DynamicInfo info in Objects)
            {
                if (info.Transform != null)
                {
                    info.Position.OnUpdate();
                    info.Rotation.OnUpdate();
                    info.Scale.OnUpdate();
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    [ContextMenu("Get To")]
    void GetTo()
    {

        Objects[IndexToTake].Position.To = Objects[IndexToTake].Transform.localPosition;
        if (Objects[IndexToTake].Rotation.To != Objects[IndexToTake].Transform.localEulerAngles)
        {
            Objects[IndexToTake].Rotation.To = Objects[IndexToTake].Transform.localEulerAngles;
            Objects[IndexToTake].Rotation.Enable = true;
        }
        Objects[IndexToTake].Scale.To = Objects[IndexToTake].Transform.localScale;
    }
}