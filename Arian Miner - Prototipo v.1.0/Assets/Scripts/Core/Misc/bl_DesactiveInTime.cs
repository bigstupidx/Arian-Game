////////////////////////////////////////////////////////////////////////////
// bl_DesactiveInTime
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class bl_DesactiveInTime : MonoBehaviour
{
    [SerializeField]private float Time = 1;
    [SerializeField]private bool FadeImage;

    void OnEnable()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        if (FadeImage)
        {
            Color c = GetComponent<Image>().color;
            c.a = 1;
            GetComponent<Image>().color = c;
        }
        yield return new WaitForSeconds(1);
        Invoke("Disable", Time);
        if (FadeImage)
        {
            GetComponent<Image>().CrossFadeAlpha(0, Time, true);
        }
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }
}