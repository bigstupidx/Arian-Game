using UnityEngine;
using UnityEngine.UI;

public class bl_StreakManager : MonoBehaviour {

    [Header("Settings")]
    [Range(0.01f,7)]public float TimeToCombo = 0.5f;
    [Range(0.01f,7)]public float TimeToHide = 0.5f;

    [Header("References")]
    [SerializeField]private Text ComboText;
    [SerializeField]private Animator Anim;

    private float time;
    private int Combo = 0;

    void OnEnable()
    {
        Arian.bl_Event.Global.AddListener<Arian.bl_GlobalEvents.OnPoint>(this.OnPickUp);
    }

    void OnDisable()
    {
        Arian.bl_Event.Global.RemoveListener<Arian.bl_GlobalEvents.OnPoint>(this.OnPickUp);
    }


    void OnPickUp(Arian.bl_GlobalEvents.OnPoint e)
    {
        if(time >= Time.time)
        {
            Combo++;
        }
        else
        {
            Combo = 0;
        }

        time = Time.time + TimeToCombo;
        if (Combo > 1)
        {
            CancelInvoke();
            ComboText.text = string.Format("COMBO x{0}", Combo);
            if(Combo == 2)
            {
                Anim.SetBool("show", true);
            }
            else
            {
                Anim.SetBool("show", true);
                Anim.Play("Pum", 0, 0);
            }
            Invoke("DisableCombo", TimeToHide);
        }
        else
        {
            Anim.SetBool("show", false);
        }

    }

    void DisableCombo()
    {
        Anim.SetBool("show", false);
    }
}