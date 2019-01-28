using UnityEngine;
using System;

[Serializable]
public class bl_LevelInfo  {
    public string Name;
    public int Speed;
    public int PointsNeeded;
    public int ObstaclesLevel;

    [Header("References")]
    public AudioClip m_AudioClip;
}