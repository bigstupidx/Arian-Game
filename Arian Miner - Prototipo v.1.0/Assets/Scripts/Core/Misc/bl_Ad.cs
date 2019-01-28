using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class bl_Ad : MonoBehaviour
{

    [SerializeField,Range(1,10)]private int ShowEachTry = 5;

    private int TrysCount;

    /// <summary>
    /// 
    /// </summary>
    public void SetTry()
    {
        TrysCount++;
        if(TrysCount >= ShowEachTry)
        {
#if UNITY_ADS
            if (Advertisement.IsReady())
            {
                Advertisement.Show();
            }
            TrysCount = 0;
#endif
        }
    }
}