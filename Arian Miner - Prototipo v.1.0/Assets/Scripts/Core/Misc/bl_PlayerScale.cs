using UnityEngine;
using System;

[Serializable]
public class bl_PlayerScale  {

    public Vector3 FlipScaleVertical;
    public Vector3 FlipScaleHorizontal;
    public Vector3 OnChangeSide;
    public Vector3 OnMoveHorizontal;
    public Vector3 OnMoveVertical;
    public Vector3 OnDeath;

    [Header("Settings")]
    public float Lerp = 8;
    public float TimeMaintain = 2.5f;

    private Vector3 DefaultScale;
    private Transform m_Transform;
    private State m_State = State.Idle;
    private bl_ChangerManager Changer;

    public void Init(Transform Player)
    {
        m_Transform = Player;
        DefaultScale = bl_GameManager.Instance.CacheDefaultPlayerScale;
        Changer = bl_ChangerManager.Instance;
    }

    public void OnUpdate()
    {
        Vector3 p = DefaultScale;
        float speed = Lerp;

        switch (m_State)
        {
            case State.Flip:
                p = (Changer.IsVertical) ? FlipScaleVertical : FlipScaleHorizontal;
                break;
            case State.Change:
                p = OnChangeSide;
                break;
            case State.Death:
                p = OnDeath;
                break;
            case State.Move:
                p = (Changer.IsVertical) ? OnMoveVertical : OnMoveHorizontal;
                speed = speed / 2;
                break;
            case State.Idle:
                p = DefaultScale;
                break;
        }

        m_Transform.localScale = Vector3.Lerp(m_Transform.lossyScale, p, Time.deltaTime * Speedbox.bl_Utils.MathfUtils.EaseLerpExtreme(speed));
    }

    public void ChangeState(State _state)
    {
        m_State = _state;
    }

    [Serializable]
    public enum State
    {
        Idle,
        Change,
        Flip,
        Move,
        Death,
    }
}