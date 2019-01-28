using UnityEngine;
using System.Collections;

public class bl_CameraTil : MonoBehaviour
{

    [Header("TillEffect")]
    private Transform m_transform;
    public float smooth = 4f;
    public float tiltAngle = 6f;
    [Header("FallEffect")]
    [Range(0.01f, 1.0f)]
    public float m_time = 0.2f;
    public float DelayDown = 0.4f;
    public float DownAmount = 8;
    private bl_PlayerController PlayerC;

    void Awake()
    {
        PlayerC = bl_PlayerController.Instance;
        this.m_transform = this.transform;
    }

    void OnEnable()
    {
        Arian.bl_Event.Global.AddListener<Arian.bl_GlobalEvents.OnChangeSide>(this.OnChange);
    }

    void OnDisable()
    {
        Arian.bl_Event.Global.RemoveListener<Arian.bl_GlobalEvents.OnChangeSide>(this.OnChange);
    }

    void Update()
    {
        float t_amount = PlayerC.MovementValue * this.tiltAngle;
        t_amount = Mathf.Clamp(t_amount, -this.tiltAngle, this.tiltAngle);
        this.m_transform.localRotation = Quaternion.Lerp(this.m_transform.localRotation, Quaternion.Euler(0, 0, t_amount), Time.deltaTime * Speedbox.bl_Utils.MathfUtils.EaseLerpExtreme(smooth));
    }

    void OnChange(Arian.bl_GlobalEvents.OnChangeSide e)
    {
        StopAllCoroutines();
        StartCoroutine(FallEffect());
    }


    IEnumerator FallEffect()
    {
        yield return new WaitForSeconds(DelayDown);
        Quaternion m_default = this.transform.localRotation;
        Quaternion m_finaly = this.transform.localRotation * Quaternion.Euler(new Vector3(DownAmount, 0, 0));
        float t_rate = 1.0f / m_time;
        float t_time = 0.0f;
        while (t_time < 1.0f)
        {
            t_time += Time.deltaTime * t_rate;
            this.transform.localRotation = Quaternion.Slerp(m_default, m_finaly, t_time);
            yield return t_rate;
        }
    }
}