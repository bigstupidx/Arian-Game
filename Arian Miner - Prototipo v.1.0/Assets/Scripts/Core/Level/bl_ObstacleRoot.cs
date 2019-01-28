using UnityEngine;
using System.Collections;

public class bl_ObstacleRoot : MonoBehaviour
{
    public Vector2 HorizontalLimit;
    public Vector2 VerticalLimit;
    [Header("References")]
    public GameObject explosion;
    [SerializeField]private GameObject PickupEffect;
    public GameObject ItemPrefab;
    [SerializeField]private bl_Obstacle[] AllObstacles;


    private bool PassPlayer = false;
    private bl_GameManager m_GameManager;
    private bl_ObstaclesManager OM;
    private bl_Obstacle ActiveObstacle;
    private float distance;

    void Awake()
    {
        m_GameManager = bl_GameManager.Instance;
        OM = bl_ObstaclesManager.Instance;
    }

    void OnEnable()
    {
        DesactiveAll();
        SelectObstacle();
    }

    void DesactiveAll()
    {
        foreach (bl_Obstacle o in AllObstacles)
        {
            o.gameObject.SetActive(false);
        }
        PickupEffect.SetActive(false);
    }

    void SelectObstacle()
    {
        PassPlayer = false;
        int i = bl_ObstaclesManager.Instance.SelectNext(AllObstacles);
        foreach (bl_Obstacle o in AllObstacles)
        {
            if (o.ID == i)
            {
                o.gameObject.SetActive(true);
                bl_ObstaclesManager.Instance.SetLastObstacle(o);
                ActiveObstacle = o;
            }
            else
            {
                o.gameObject.SetActive(false);
            }
        }
    }

    public void OnPickUp(Transform t)
    {
        PickupEffect.transform.position = t.position;
        PickupEffect.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        Move();
        CalculateDistance();
        CalculateWhenPass();
    }

    /// <summary>
    /// 
    /// </summary>
    void Move()
    {
        transform.Translate(OM.SpeedMultiplier * Vector3.back * m_GameManager.ScrollSpeed * Time.deltaTime * 5f);
    }

    /// <summary>
    /// 
    /// </summary>
    void CalculateDistance()
    {
        distance = transform.position.z - Camera.main.transform.position.z;
        if (distance < 10)
        {
            PassPlayer = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void CalculateWhenPass()
    {
        if (PassPlayer && distance < 1)
        {

            if (OM.UsePoolMethod)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    public void DoExplosion(Transform t)
    {
        GameObject go = Instantiate(explosion) as GameObject;
        go.transform.position = t.position;
        Destroy(go, 5);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Destroy()
    {
        if (OM.UsePoolMethod)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bl_Obstacle TheObstacle
    {
        get
        {
            return ActiveObstacle;
        }
    }

 #if UNITY_EDITOR
    [ContextMenu("Set ID's")]
    void SetIDS()
    {
        bl_Obstacle[] all = GetComponentsInChildren<bl_Obstacle>(true);
        int i = 0;
        foreach(bl_Obstacle obs in all)
        {
            obs.ID = i;
            i++;
        }
    }

    [ContextMenu("Get All")]
    void GetAllObs()
    {
        AllObstacles = GetComponentsInChildren<bl_Obstacle>(true);
    }
#endif

}