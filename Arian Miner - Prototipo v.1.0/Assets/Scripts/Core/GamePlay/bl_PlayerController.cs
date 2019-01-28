using UnityEngine;
using System.Collections;

public class bl_PlayerController : Singleton<bl_PlayerController>
{
    [Header("Settings")]
    public float MoveSpeed = 2;
    public float FlipLerp = 6;
    public Vector2 HorizontalClamp;
    public Vector2 VerticalClamp;

    [Header("Scale Settings")]
    public bool UseScale = true;
    public bl_PlayerScale m_Scale = new bl_PlayerScale();
    public Transform m_Transform;
    private float MoveDirection = 0;
   [SerializeField] private bool CanControll = false;
    private bl_ChangerManager ChangerManager;
    private float buttonRate;
    private bool isRegister = false;
    private Animator m_Anim;
    public Animator m_AnimSpaceship;
    public GameObject spaceship;
    public bool correctRotation = false;

    void Awake()
    {
        ChangerManager = bl_ChangerManager.Instance;
        m_Transform = GetComponent<Transform>();
        m_Anim = GetComponent<Animator>();
        //QualitySettings.antiAliasing = 3;
    }

    void Start()
    {
        m_Scale.Init(transform);
        m_AnimSpaceship.enabled = false;
    }

    void InitSequence()
    {
        CanControll = false;
        m_Anim.enabled = true;
        StopCoroutine("WaitInit");
        StartCoroutine("WaitInit");
    }

    IEnumerator WaitInit()
    {
        yield return new WaitForSeconds(0.5f);
        m_Anim.Play("InitGame", 0, 0);
        yield return new WaitForSeconds(m_Anim.GetCurrentAnimatorClipInfo(0).Length);
        m_Anim.enabled = false;
        CanControll = true;
        StartSpaceshipAnim();
    }

    public void StartSpaceshipAnim()
    {
        correctRotation = true;
        m_AnimSpaceship.enabled = true;
        m_AnimSpaceship.Play("Idle");
    }

    void OnEnable()
    {
        if (!isRegister)
        {
            Arian.bl_Event.Global.AddListener<Arian.bl_GlobalEvents.OnStartPlay>(OnStartPlay);
            Arian.bl_Event.Global.AddListener<Arian.bl_GlobalEvents.OnFailGame>(OnFailGame);
            Arian.bl_Event.Global.AddListener<Arian.bl_GlobalEvents.OnPoint>(OnPoint);
            Arian.bl_Event.Global.AddListener<Arian.bl_GlobalEvents.OnChangeSide>(OnChangeSide);
            isRegister = true;
        }
    }

    void OnDestroy()
    {
        Arian.bl_Event.Global.RemoveListener<Arian.bl_GlobalEvents.OnStartPlay>(OnStartPlay);
        Arian.bl_Event.Global.RemoveListener<Arian.bl_GlobalEvents.OnFailGame>(OnFailGame);
        Arian.bl_Event.Global.RemoveListener<Arian.bl_GlobalEvents.OnPoint>(OnPoint);
        Arian.bl_Event.Global.RemoveListener<Arian.bl_GlobalEvents.OnChangeSide>(OnChangeSide);
    }

    void OnStartPlay(Arian.bl_GlobalEvents.OnStartPlay e)
    {
        StopAllCoroutines();
        CanControll = true;
        InitSequence();
    }

    void OnFailGame(Arian.bl_GlobalEvents.OnFailGame e)
    {
        MoveDirection = 0;
        CanControll = false;
        gameObject.SetActive(false);
    }

    void OnPoint(Arian.bl_GlobalEvents.OnPoint e)
    {
      // Animation here!
    }

    void OnChangeSide(Arian.bl_GlobalEvents.OnChangeSide e)
    {
        if (!e.byFlip)
        {
            ChangeScaleState(bl_PlayerScale.State.Change);
        }
    }
    
    void Update()
    {
        if (CanControll)
        {
            MoveControl();
            Move();
            ClampPosition();
        }
        else { MoveDirection = 0; }

        if (UseScale)
        {
            m_Scale.OnUpdate();
        }

        //SpaceShipRotate();
    }
    void MoveControl()
    {
        if (InputLeftKey)
        {
            MoveDirection = -1;
            ChangeScaleState(bl_PlayerScale.State.Move);
        }
        if (InputRightKey)
        {
            MoveDirection = 1;
            ChangeScaleState(bl_PlayerScale.State.Move);
        }
        if (InputUpKeys)
        {
            MoveDirection = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Flip();
        }
    }

    void Flip()
    {
        bl_ChangerManager.Instance.Flip();
        ChangeScaleState(bl_PlayerScale.State.Flip);
    }
    void Move()
    {
        if (MoveDirection == 0)
            return;

        Vector3 p = m_Transform.position;
        switch (ChangerManager.Side)
        {
            case LevelSides.Down:
                p.x += MoveDirection * (MoveSpeed * Time.timeScale);
                break;
            case LevelSides.Left:
                p.y -= MoveDirection * (MoveSpeed * Time.timeScale);
                break;
            case LevelSides.Right:
                p.y += MoveDirection * (MoveSpeed * Time.timeScale);
                break;
            case LevelSides.Up:
                p.x -= MoveDirection * (MoveSpeed * Time.timeScale);
                break;
        }

        m_Transform.position = p;
    
    }
    void ClampPosition()
    {
        Vector3 p = m_Transform.position;
        p.x = Mathf.Clamp(p.x, HorizontalClamp.x, HorizontalClamp.y);
        p.y = Mathf.Clamp(p.y, VerticalClamp.x, VerticalClamp.y);
        m_Transform.position = p;
    }


    public void DoChanger(bool smooth = false)
    {
        if (!smooth)
        {
            SetYOffSet(ChangerManager.GetSideInfo.YOffSet);
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(SetYOffSetSmooth(ChangerManager.GetSideInfo.YOffSet));
        }
    }
    IEnumerator SetYOffSetSmooth(float value)
    {
        Vector3 p = m_Transform.position;
        switch (ChangerManager.Side)
        {
            case LevelSides.Down:
                p.y = value;
                break;
            case LevelSides.Left:
                p.x = value;
                break;
            case LevelSides.Right:
                p.x = value;
                break;
            case LevelSides.Up:
                p.y = value;
                break;
        }
        while(Vector3.Distance(m_Transform.position,p) > 0.005f)
        {
            m_Transform.position = Vector3.MoveTowards(m_Transform.position, p, Time.deltaTime * FlipLerp);
            yield return null;
        }
        m_Transform.position = p;
    }
    private void SetYOffSet(float value)
    {
        Vector3 p = m_Transform.position;
        switch (ChangerManager.Side)
        {
            case LevelSides.Down:
                p.y = value;
                break;
            case LevelSides.Left:
                p.x = value;
                //if(m_Transform.position.x > 0)  p.y += -.5f;
                //else p.y += .5f;
                break;
            case LevelSides.Right:
                p.x = value;
                break;
            case LevelSides.Up:
                p.y = value;
                break;
        }
        m_Transform.position = p;
    }
    public void ChangeScaleState(bl_PlayerScale.State state)
    {
        if (state == bl_PlayerScale.State.Move)
            return;
            
        StopCoroutine("ChangeScaleStateIE");
        StartCoroutine("ChangeScaleStateIE",state);
    }
    IEnumerator ChangeScaleStateIE(bl_PlayerScale.State state)
    {
        m_Scale.ChangeState(state);
        yield return new WaitForSeconds(m_Scale.TimeMaintain);
        m_Scale.ChangeState(bl_PlayerScale.State.Idle);
    }

    IEnumerator DeathSecuence() // Checked
    {
        ChangeScaleState(bl_PlayerScale.State.Death);
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    private bool InputRightKey // Checked
    {
        get
        {
            if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                return true;
            }
            return false;
        }
    }

    private bool InputLeftKey // Checked
    {
        get
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                return true;
            }
            return false;
        }
    }

    private bool InputUpKeys // Checked
    {
        get
        {
            if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                return true;
            }
            return false;
        }
    }

    public float MovementValue
    {
        get
        {
            return MoveDirection;
        }
    }

    public static bl_PlayerController Instance
    {
        get
        {
            return ((bl_PlayerController)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }
}