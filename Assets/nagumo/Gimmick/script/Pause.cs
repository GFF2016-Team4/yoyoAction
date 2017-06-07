using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {

    public GameObject m_pauseCanvas;
    public GameObject m_canvas;
    public Camera m_camera;
    [Header("タイトルシーン名")]
    public string m_title;
    [Header("ステージ選択シーン名")]
    public string m_stageSelect;

    string nowScene;
    bool m_pause;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        nowScene = SceneManager.GetActiveScene().name;

        if (m_pause == false && Input.GetKeyDown(KeyCode.P))
        {
            m_pause = true;
            Time.timeScale = 0;
            m_pauseCanvas.SetActive(true);
            m_camera.GetComponent<playerCamera>().enabled = false;
        }
        else if (m_pause == true && Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 1;
            m_pauseCanvas.SetActive(false);
            m_camera.GetComponent<playerCamera>().enabled = true;
            m_pause = false;
        }

        //if (Input.GetKeyDown(KeyCode.G))
        //    m_canvas.GetComponent<FadeManager>().SceneChange("GameOver");
	}

    public void StratButtonPush()
    {
        Time.timeScale = 1;
        m_pauseCanvas.SetActive(false);
        m_camera.GetComponent<playerCamera>().enabled = true;   
    }
    public void ReStratButtonPush()
    {
        Time.timeScale = 1;
        m_pauseCanvas.SetActive(false);
        m_canvas.GetComponent<FadeManager>().SceneChange(nowScene);
    }
    public void TitleButtonPush()
    {
        Time.timeScale = 1;
        m_pauseCanvas.SetActive(false);
        m_canvas.GetComponent<FadeManager>().SceneChange(m_title);
    }
    public void StageSelect()
    {
        Time.timeScale = 1;
        m_pauseCanvas.SetActive(false);
        m_canvas.GetComponent<FadeManager>().SceneChange(m_stageSelect);
    }
}
