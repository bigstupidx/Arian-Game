using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Speedbox;

public class bl_Stylish : Singleton<bl_Stylish> {

    public List<Palette> Palettes = new List<Palette>();
    public bool ApplyNewStyles = true;
    [Header("References")]
    [SerializeField]private Material WallHorizonatalMat;
    [SerializeField]private Material WallVerticalMat;
    [SerializeField]private Material ObstaclesMat;
    [SerializeField]private Material PlayerMat;
    [SerializeField]private Material ItemMat;
    [SerializeField]private Text LevelText;
    [SerializeField]private Image FillImage;
    [SerializeField]private Image FillImageBack;

    [SerializeField]private int SetID;

    public void ChangeStyle(int id)
    {
        if (!ApplyNewStyles)
        {
            //if (id != 0)
                return;
        }

        PlayerMat.SetColor("_Color", Palettes[id].PlayerColor);
        PlayerMat.SetColor("_RimColor", Palettes[id].PlayerRimColor);

        WallHorizonatalMat.SetColor("_Color", Palettes[id].WallHorizontal);
        WallHorizonatalMat.SetColor("_RimColor", Palettes[id].WallHorizontalRim);
        WallVerticalMat.SetColor("_Color", Palettes[id].WallVertical);
        WallVerticalMat.SetColor("_RimColor", Palettes[id].WallVerticalRim);
        ObstaclesMat.SetColor("_Color", Palettes[id].Obstacles);
        ObstaclesMat.SetColor("_RimColor", Palettes[id].Obstacles);
        ItemMat.SetColor("_Color", Palettes[id].ItemColor);

        LevelText.color = Palettes[id].LevelText;
        FillImage.color = Palettes[id].FillImageColor;
        FillImage.GetComponent<Speedbox.bl_TextGradient>().StartColor = Palettes[id].FillImageColor;
        FillImage.GetComponent<Speedbox.bl_TextGradient>().EndColor = Palettes[id].FillImageColorBottom;
        FillImageBack.color = Palettes[id].FillImageColor;
    }

    [ContextMenu("Set ID")]
    void Set()
    {
        ChangeStyle(SetID);
    }


    [System.Serializable]
    public class Palette
    {
        [Header("Player")]
        public Color PlayerColor;
        public Color PlayerRimColor;
        [Header("Level")]
        public Color WallHorizontal;
        public Color WallHorizontalRim;
        public Color WallVertical;
        public Color WallVerticalRim;
        public Color Obstacles;
        public Color ItemColor;
        [Header("UI")]
        public Color LevelText;
        public Color FillImageColor;
        public Color FillImageColorBottom;
    }

    public static bl_Stylish Instance
    {
        get
        {
            return ((bl_Stylish)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }
}