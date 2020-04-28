using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvent : MonoBehaviour
{

    //public GameObject button;


    public void OnClickExit()
    {
        Application.Quit();
        Debug.Log("Button Click - 게임종료");
    }


    public void SceneChangeStart()
    {
        SceneManager.LoadScene("Start Scene");
        Debug.Log("씬 이동");

    }
    public void SceneChangeLogin()
    {
        SceneManager.LoadScene("Login Scene");
        Debug.Log("씬 이동");

    }
    public void SceneChangeSettings()
    {
        SceneManager.LoadScene("Settings Scene");
        Debug.Log("씬 이동");

    }
    public void SceneChangeExplain()
    {
        SceneManager.LoadScene("Explain Scene");
        Debug.Log("씬 이동");

    }
}
