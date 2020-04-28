using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickExit()
    {
        Application.Quit();
        Debug.Log("Button Click - 게임종료");
    }

    public void ChangeLoginScene()
    {
        SceneManager.LoadScene("LoginScene");
        Debug.Log("씬 이동");
    }

    public void ChangeMainScene()
    {
        SceneManager.LoadScene("StartScene");
        Debug.Log("씬 이동");
    }

    public void ChangeSettingsScene()
    {
        SceneManager.LoadScene("SettingsScene");
        Debug.Log("씬 이동");
    }

    public void ChangeExplainScene()
    {
        SceneManager.LoadScene("ExplainScene");
        Debug.Log("씬 이동");
    }


}
