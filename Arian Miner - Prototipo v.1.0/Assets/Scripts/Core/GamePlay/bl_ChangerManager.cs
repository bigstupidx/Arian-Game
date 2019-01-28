using UnityEngine;
using System.Collections.Generic;

public class bl_ChangerManager : Singleton<bl_ChangerManager> {

    [Header("Settings")]
    public List<bl_SidesInfo> Sides = new List<bl_SidesInfo>();
    [Header("References")]
    [SerializeField]private Transform LevelRoot;

    private bl_SidesInfo CurrentSide;
    public Transform stabilizer;

    public bl_PlayerController PlayerController;

    void Awake()
    {
        //PlayerController = GetComponent<bl_PlayerController>();
        CurrentSide = Sides[3];
    }

    public void Change(bl_ChangeDetector detector)
    {
        CurrentSide = GetSide(detector);
        new Arian.bl_GlobalEvents.OnChangeSide(CurrentSide.Side,false);

        bl_PlayerController.Instance.DoChanger();
        CorrectPlayerAngle(CurrentSide.Side);
    }

    public void CorrectPlayerAngle(LevelSides side)
    {
        if(PlayerController.correctRotation)
        {
            switch (side)
            {
                case LevelSides.Left: 
                    stabilizer.eulerAngles = new Vector3(0, 0, -90);
                    break;
                case LevelSides.Right: 
                    stabilizer.eulerAngles = new Vector3(0, 0, 90);
                    break;
                case LevelSides.Up: 
                    stabilizer.eulerAngles = new Vector3(0, 0, 180);
                    break;
                case LevelSides.Down: 
                    stabilizer.eulerAngles = new Vector3(0, 0, 0);
                    break;
                default:
                    stabilizer.eulerAngles = new Vector3(0, 0, 0);
                    break;
            }
        }
    }

    public void Change(LevelSides side,bool isFlip,bool smooth = false)
    {
        CurrentSide = GetSide(side);
        new Arian.bl_GlobalEvents.OnChangeSide(side,isFlip);
        bl_PlayerController.Instance.DoChanger(smooth);
    }

    public void Flip()
    {
        Change(GetSideInfo.Oposite,true,true);
    }

    private bl_SidesInfo GetSide(bl_ChangeDetector de)
    {
        for(int i = 0; i < Sides.Count; i++)
        {
            if(Sides[i].Side == de.Side)
            {
                return Sides[i];
            }
        }
        return Sides[0];
    }

    private bl_SidesInfo GetSide(LevelSides sid)
    {
        for (int i = 0; i < Sides.Count; i++)
        {
            if (Sides[i].Side == sid)
            {
                return Sides[i];
            }
        }
        return Sides[0];
    }

    public LevelSides Side
    {
        get
        {
            return CurrentSide.Side;
        }
    }

    public bl_SidesInfo GetSideInfo
    {
        get
        {
            return CurrentSide;
        }
    }

    public bool IsVertical
    {
        get
        {
            return (Side == LevelSides.Down || Side == LevelSides.Up);
        }
    }

    public static bl_ChangerManager Instance
    {
        get
        {
            return ((bl_ChangerManager)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }
}