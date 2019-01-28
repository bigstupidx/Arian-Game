using UnityEngine;
using System;

[Serializable]
public class bl_SidesInfo
{
    public LevelSides Side;
    public LevelSides Oposite;
    [Header("Level")]
    public Vector3 LevelPosition;
    public Vector3 LevelRotation;
    [Header("Player")]
    public float YOffSet;
}