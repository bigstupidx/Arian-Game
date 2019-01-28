using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bl_ObstaclesManager : Singleton<bl_ObstaclesManager>
{
    [Header("Obstacles")]
    public bool StartObstacles = true;
    public bool UsePoolMethod = true;
    public float SpeedMultiplier = 2;
    public int SpaceBetweenSecuences = 4;
    public int SpaceBetweenSecuencesInfinity = 4;
    public int maxObstacles = 5;
    public float maxWaitBetweenObstacles = 2f;
    public float maxWaitBetweenObstaclesInfinity = 2f;
    public int ProgresivePointsPerObstacles = 5;

    [Header("References")]
    [SerializeField]private GameObject[] ObstaclePrefabs;
    [SerializeField]private GameObject ItemPrefab;
    [SerializeField]private Transform SpawnObtacles;
    [SerializeField]private Transform ItemParent;

    private bl_Obstacle LastObstacle;
    private bool isGameOver = true;
    private int Score;
    private List<int> SecuenceArray = new List<int>();
    private int CurrentSecuence;
    private bool isInSecuence = false;
    private int DefaultSpaceBetwwenSecuences;
    private List<GameObject> PoolArray = new List<GameObject>();
    private int CurrenPoolObject;
    private List<GameObject> PoolItemArray = new List<GameObject>();
    private int CurrenItemObject;
    private bl_GameManager m_GameManager = null;
   

    void Awake()
    {
        m_GameManager = bl_GameManager.Instance;        
        DefaultSpaceBetwwenSecuences = CurrentSecuence - SpaceBetweenSecuencesInfinity;
    }

    void Start()
    {
        if (UsePoolMethod)
        {
            for(int i = 0; i < maxObstacles * 2; i++)
            {
                GameObject pol = Instantiate(ObstaclePrefabs[0]) as GameObject;
                pol.transform.parent = SpawnObtacles;
                PoolArray.Add(pol);
                pol.SetActive(false);
            }

            for (int i = 0; i < 10; i++)
            {
                GameObject pol = Instantiate(ItemPrefab) as GameObject;
                pol.transform.parent = ItemParent;
                PoolItemArray.Add(pol);
                pol.SetActive(false);
            }
        }
    }

    void OnEnable()
    {
        Arian.bl_Event.Global.AddListener<Arian.bl_GlobalEvents.OnStartPlay>(OnStartPlay);
        Arian.bl_Event.Global.AddListener<Arian.bl_GlobalEvents.OnFailGame>(OnFailGame);
        Arian.bl_Event.Global.AddListener<Arian.bl_GlobalEvents.OnPoint>(OnPoint);
    }

    void OnDisable()
    {
        Arian.bl_Event.Global.RemoveListener<Arian.bl_GlobalEvents.OnStartPlay>(OnStartPlay);
        Arian.bl_Event.Global.RemoveListener<Arian.bl_GlobalEvents.OnFailGame>(OnFailGame);
        Arian.bl_Event.Global.RemoveListener<Arian.bl_GlobalEvents.OnPoint>(OnPoint);
    }

    void OnStartPlay(Arian.bl_GlobalEvents.OnStartPlay e)
    {
        RemoveAllObstacles();
        StopAllCoroutines();
        isGameOver = false;
        if (StartObstacles)
        {
            StartCoroutine(SpawnSecunce());
        }
    }

    public GameObject GetItem()
    {
        CurrenItemObject = (CurrenItemObject + 1) % 10;
        return PoolItemArray[CurrenItemObject];
    }

    void OnFailGame(Arian.bl_GlobalEvents.OnFailGame e)
    {
        isGameOver = true;
        StopAllCoroutines();
    }

    void OnPoint(Arian.bl_GlobalEvents.OnPoint e)
    {
        Score = e.Point;
        if (!isInSecuence)
        {
            CurrentSecuence--;
        }
    }

    void RemoveAllObstacles()
    {
        bl_ObstacleRoot[] all = FindObjectsOfType<bl_ObstacleRoot>();
        if (all == null || all.Length <= 0)
            return;

        foreach (bl_ObstacleRoot o in all)
        {
            o.Destroy();
        }
    }

    IEnumerator SpawnSecunce()
    {
        yield return new WaitForSeconds(1.5f);
        GameObject cacheObject = null;
        while (true)
        {
            if (UsePoolMethod)
            {
                CurrenPoolObject = (CurrenPoolObject + 1) % PoolArray.Count;
                cacheObject = PoolArray[CurrenPoolObject];
                cacheObject.transform.position = SpawnObtacles.position;
                cacheObject.transform.localRotation = Quaternion.identity;
                cacheObject.SetActive(true);

                float wbo = maxWaitBetweenObstacles;
                float waitTime = Mathf.Max(wbo - (Score / 10f), wbo / 3f);
                waitTime += (LastObstacle.ExtraSpaceTime / 10);

                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                GameObject go = Instantiate(ObstaclePrefabs[m_GameManager.GetCurrentObstacleLevel]) as GameObject;
                go.transform.position = SpawnObtacles.position;
                go.transform.parent = SpawnObtacles;
                go.transform.localRotation = Quaternion.identity;
                float wbo = maxWaitBetweenObstacles;
                yield return new WaitForSeconds(Mathf.Max(wbo - (Score / 10f), wbo / 3f));

                while (maxObstacles <= SpawnObtacles.childCount)
                {
                    yield return null;
                }
            }


            if (isGameOver)
                break;

            yield return null;
        }
    }

    public int SelectNext(bl_Obstacle[] all)
    {
        int next = 0;
        if (isInSecuence)
        {
            CurrentSecuence++;
            if (CurrentSecuence < SecuenceArray.Count - 1)
            {
                next = SecuenceArray[CurrentSecuence];
            }
            else
            {
                CurrentSecuence = 0;
                isInSecuence = false;
            }
        }
        else
        {
            if (CanSecuence && LastObstacle != null && LastObstacle.UseSecuence)
            {
                GetNextSecuence(ref next, all);
            }
            else
            {
                GetNextNormal(ref next, all);
            }
        }

        return next;
    }

    private void GetNextSecuence(ref int next, bl_Obstacle[] all)
    {
        int posibilities = Random.Range(0, LastObstacle.Possibilities);
        bool WithSecuence = (posibilities == 0);

        if (WithSecuence)
        {
            SecuenceArray.Clear();
            SecuenceArray.AddRange(LastObstacle.Secuence);
            CurrentSecuence = 0;
            next = SecuenceArray[CurrentSecuence];
        }
        else
        {
            GetNextNormal(ref next, all);
        }
    }

    private void GetNextNormal(ref int next, bl_Obstacle[] all)
    {
        List<int> AllIDs = new List<int>();
        foreach (bl_Obstacle o in all)
        {
            AllIDs.Add(o.ID);
        }

        if (LastObstacle != null)
        {
            if (LastObstacle.ApplyRejects)
            {
                //exclude all id where we not need for the next obtacle
                //based in the reject list of last obstacle.
                List<int> excludes = new List<int>();
                excludes.AddRange(LastObstacle.Rejects);
                for (int i = 0; i < excludes.Count; i++)
                {
                    if (AllIDs.Contains(excludes[i]))
                    {
                        AllIDs.Remove(excludes[i]);
                    }
                }
                //remove the last id from the list for not repeat the same obstacle
                if (AllIDs.Contains(LastObstacleID))
                {
                    AllIDs.Remove(LastObstacleID);
                }
                next = AllIDs[Random.Range(0, AllIDs.Count)];
            }
            else
            {
                //remove the last id from the list for not repeat the same obstacle
                if (AllIDs.Contains(LastObstacleID))
                {
                    AllIDs.Remove(LastObstacleID);
                }
                next = AllIDs[Random.Range(0, AllIDs.Count)];
            }
        }
        else
        {
            next = AllIDs[Random.Range(0, AllIDs.Count)];
        }
    }

    public void SetLastObstacle(bl_Obstacle obs)
    {
        LastObstacle = obs;
    }

    public int LastObstacleID
    {
        get
        {
            return LastObstacle.ID;
        }
    }

    private bool CanSecuence
    {
        get
        {
            return CurrentSecuence <= DefaultSpaceBetwwenSecuences;
        }
    }

    public static bl_ObstaclesManager Instance
    {
        get
        {
            return ((bl_ObstaclesManager)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }
}