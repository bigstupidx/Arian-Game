////////////////////////////////////////////////////////////////////////////
// bl_DynamicInfo
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System;

[Serializable]
public class bl_DynamicInfo 
{
    public Transform Transform;
    public bl_PositionDynamic Position;
    public bl_RotationDynamic Rotation;
    public bl_ScaleDynamic Scale;
   /* [InspectorButton("GetTo", "Get To")]
    public int Button;

    void GetTo()
    {
        Position.To = Transform.localPosition;
        Rotation.To = Transform.localEulerAngles;
        Scale.To = Transform.localScale;
    }*/
}