using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bl_Shaker : Singleton<bl_Shaker>
{
    [Header("Shake")]
    public Transform ShakeObject = null;
    private Vector3 originPosition;
    private Quaternion originRotation;
    private float shakeIntensity;
    [Header("Presents")]
    public List<Info> ShakesPresents = new List<Info>();

    void Start()
    {
        originPosition = ShakeObject.localPosition;
        originRotation = ShakeObject.localRotation;
    }

    public void Do()
    {
        StopAllCoroutines();
        StartCoroutine(Shake(ShakesPresents[0]));
    }

    public void Do(int index)
    {
        StopAllCoroutines();
        StartCoroutine(Shake(ShakesPresents[index]));
    }

    public void Do(Info inf)
    {
        StopAllCoroutines();
        StartCoroutine(Shake(inf));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator Shake(Info _info)
    {
        yield return new WaitForSeconds(_info.Delay);
        shakeIntensity = _info.ShakeIntensity;
        while (shakeIntensity > 0)
        {
            ShakeObject.localPosition = originPosition + Random.insideUnitSphere * shakeIntensity;
            ShakeObject.localRotation = new Quaternion(
                originRotation.x + Random.Range(-shakeIntensity, shakeIntensity) * _info.ShakeAmount,
                originRotation.y + Random.Range(-shakeIntensity, shakeIntensity) * _info.ShakeAmount,
                originRotation.z + Random.Range(-shakeIntensity, shakeIntensity) * _info.ShakeAmount,
                originRotation.w + Random.Range(-shakeIntensity, shakeIntensity) * _info.ShakeAmount);
            shakeIntensity -= _info.ShakeDecay;
            yield return false;
        }
        ShakeObject.localPosition = originPosition;
        ShakeObject.localRotation = originRotation;
    }

    public static bl_Shaker Instance
    {
        get
        {
            return ((bl_Shaker)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }

    [System.Serializable]
    public class Info
    {
        public string Name = "Shake Name";
        [Range(0.001f, 0.01f)]
        public float ShakeDecay = 0.002f;
        [Range(0.01f, 0.2f)]
        public  float ShakeIntensity = 0.02f;
        [Range(0.01f, 0.5f)]
        public float ShakeAmount = 0.2f;
        [Range(0f, 5f)]
        public float Delay = 0f;
    }
}