using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bl_ColorChanger : MonoBehaviour {

    public List<Color> ColorArray = new List<Color>();
    public bool StartInAwake = true;
    [Range(1, 10)]public float Lerp = 5;
    public float ColorRate = 3;

    private Color TargetColor;
    private MeshRenderer m_MeshRender;

    void OnEnable()
    {
        m_MeshRender = GetComponent<MeshRenderer>();
        if (StartInAwake)
        {
            InvokeRepeating("NextColor", 1, ColorRate);
            StartCoroutine(ColorBeteewn());
        }
    }

    void OnDisable()
    {
        CancelInvoke();
    }
    
    void NextColor()
    {
        TargetColor = ColorArray[Random.Range(0, ColorArray.Count)];
    }

    IEnumerator ColorBeteewn()
    {
        while (true)
        {
            if(m_MeshRender != null)
            {
                Color c = m_MeshRender.material.color;
                c = Color.Lerp(c, TargetColor, Time.deltaTime * Lerp);
                m_MeshRender.material.SetColor("_Color", c);
                m_MeshRender.material.SetColor("_RimColor", c);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}