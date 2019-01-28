using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bl_MessageBoard : MonoBehaviour
{
    [SerializeField] private string[] messages;
    public Text txt_Title;
    [SerializeField] private Color[] _colors;
    public Image img_Background;
    [SerializeField] private AudioClip[] ac_Clips;

    public void SetMessageBoard(MiningStates s)
    {
        int index = (int)s;
        txt_Title.text = messages[index];
        img_Background.color = _colors[index];
        AudioSource.PlayClipAtPoint(ac_Clips[index], transform.position);
    }
}
