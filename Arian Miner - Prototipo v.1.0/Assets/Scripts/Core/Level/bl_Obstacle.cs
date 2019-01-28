using UnityEngine;

public class bl_Obstacle : MonoBehaviour
{
    public int ID;
    [Header("Seeting")]
    public float ExtraSpaceTime = 0;
    [Header("Secuences")]
    public bool UseSecuence = false;
    public int[] Secuence;
    public int Possibilities = 2;//? of 1
    [Header("Rejects")]
    public bool ApplyRejects = false;
    public int[] Rejects;

    private bl_ObstacleRoot m_Root;
    private Vector3 InstancePosition;
    private GameObject cacheObject;

    void Start()
    {
        m_Root = GetComponentInParent<bl_ObstacleRoot>();

        int maxTrys = 12;
        CalculatePosition();
        bool found = true;

        while (Physics.CheckBox(InstancePosition, new Vector3(2, 2, 2)))
        {
            CalculatePosition();
            maxTrys--;
            if (maxTrys <= 0)
            {
                found = false;
                break;
            }
        }
        
        if (found)
        {
            if (bl_ObstaclesManager.Instance.UsePoolMethod)
            {
                cacheObject = bl_ObstaclesManager.Instance.GetItem();
                cacheObject.transform.position = InstancePosition;
                cacheObject.transform.parent = transform;
                cacheObject.SetActive(true);
            }
            else
            {
                GameObject obj = Instantiate(m_Root.ItemPrefab, InstancePosition, Quaternion.identity) as GameObject;
                obj.transform.parent = transform;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void CalculatePosition()
    {
        LevelSides s = GetRandomSide();

        switch (s)
        {
            case LevelSides.Down:
                InstancePosition =
new Vector3(Random.Range(m_Root.HorizontalLimit.x, m_Root.HorizontalLimit.y),
m_Root.VerticalLimit.x,
transform.position.z);
                break;
            case LevelSides.Up:
                InstancePosition =
new Vector3(Random.Range(m_Root.HorizontalLimit.x, m_Root.HorizontalLimit.y),
m_Root.VerticalLimit.y,
transform.position.z);
                break;
            case LevelSides.Left:
                InstancePosition =
new Vector3(m_Root.HorizontalLimit.x,
Random.Range(m_Root.VerticalLimit.x, m_Root.VerticalLimit.y),
transform.position.z);
                break;
            case LevelSides.Right:
                InstancePosition =
new Vector3(m_Root.HorizontalLimit.y,
Random.Range(m_Root.VerticalLimit.x, m_Root.VerticalLimit.y),
transform.position.z);
                break;
        }
    }


    private LevelSides GetRandomSide()
    {
        int r = Random.Range(0, 3);
        return (LevelSides)r;
    }
}