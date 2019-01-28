using UnityEngine;
using System.Collections;

public class bl_SlowMotion : Singleton<bl_SlowMotion>
{
    [Header("Settins")]
    [Range(0,1)]public float SlowTarget = 0.3f;
    public float slowTimeAllowed = 2.0f;
    [Header("Audio")]
    [SerializeField]private AudioSource Source;
    [SerializeField]private AudioClip SlowInAudio;
    [SerializeField]private AudioClip SlowOutAudio;

    private float currentSlowMo = 0;
    private bool isAuto = false;

    void Update()
    {
        if (Speedbox.bl_Utils.IsMobile)
            return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (Time.timeScale == 1.0)
            {
                Time.timeScale = SlowTarget;
            }
            else
            {
                Time.timeScale = 1.0f;
            }

            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        if (!isAuto)
        {
            if (Time.timeScale == SlowTarget)
            {
                currentSlowMo += Time.deltaTime;
            }

            if (currentSlowMo > slowTimeAllowed)
            {
                currentSlowMo = 0;
                Time.timeScale = 1.0f;
            }
        }
    }
    
    public void DoSlow(float duration,float TimeScale = 0.2f,bool shake = false)
    {
        StopAllCoroutines();
        new Arian.bl_GlobalEvents.OnSlowMotion(duration);
        StartCoroutine(WaitDuration(duration, TimeScale,shake));
    }

    IEnumerator WaitDuration(float d,float ts,bool shake)
    {
        isAuto = true;
        if (SlowInAudio) { Source.PlayOneShot(SlowInAudio); }
        Time.timeScale = ts;
        yield return StartCoroutine(Speedbox.bl_Utils.CorrutinesUtils.WaitForRealSeconds(d));
        Time.timeScale = 1.0f;
        if (SlowOutAudio) { Source.PlayOneShot(SlowOutAudio); }
        isAuto = false;
        if (shake) { bl_Shaker.Instance.Do(3); }
    }

    public static bl_SlowMotion Instance
    {
        get
        {
            return ((bl_SlowMotion)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }
}