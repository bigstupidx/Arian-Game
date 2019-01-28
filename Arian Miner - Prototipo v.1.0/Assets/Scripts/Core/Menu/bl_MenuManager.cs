using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class bl_MenuManager : Singleton<bl_MenuManager>
{   
    public void LoadScene()
    {
        StartCoroutine(WaitToLoad());
    }

    IEnumerator WaitToLoad()
    {
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}