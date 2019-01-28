using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bl_Timer : MonoBehaviour
{
    public Image timerBar;
    public GameObject timeUpText;
    public float maxTime = 60f;

    private float timeLeft;
    private bl_UIManager UIManager;

    void Start()
    {
        UIManager = this.GetComponent<bl_UIManager>();
        timeUpText.SetActive(false);
        timeLeft = maxTime;
    }

    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerBar.fillAmount = timeLeft / maxTime;
        }
        else
        {
            timeUpText.SetActive(true);
            Time.timeScale = 0;

            //UIManager.OnPlayAgain();
        }
    }
}
