using UnityEngine;
using UnityEngine.UI;

public class bl_Showwave : Singleton<bl_Showwave> {

    [SerializeField]private Sprite Vignette;
    [SerializeField]private Sprite Full;

    private Animator Anim;
    private Image m_Image;

    void Awake()
    {
        Anim = GetComponent<Animator>();
        m_Image = GetComponent<Image>();
    }

    public void Play(string parameter)
    {
        Play(parameter, m_Image.color);
    }

    public void Play(string parameter,Color c)
    {
        Play(parameter, c, Type.Vignette);
    }

    public void Play(string parameter, Color c,Type type)
    {
        m_Image.color = c;
        switch (type)
        {
            case Type.Full:
                m_Image.sprite = Full;
                break;
            case Type.Vignette:
                m_Image.sprite = Vignette;
                break;
        }
        Anim.Play(parameter, 0, 0);
    }

    public static bl_Showwave Instance
    {
        get
        {
            return ((bl_Showwave)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }

    [System.Serializable]
    public enum Type
    {
        Vignette,
        Full,
    }
}