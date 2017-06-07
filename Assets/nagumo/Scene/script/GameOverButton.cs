using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverButton : MonoBehaviour {

    [Header("フェード用Canvas")]
    public GameObject m_canvas;
    public GameObject m_eventSystem;
    [Header("それぞれのステージ名")]
    public string m_stage1;
    public string m_stage2;
    public string m_stage3;
    public string m_stage4;
    public string m_stage5;

    [Header("タイトルシーン名")]
    public string m_titleName;

    string m_retryScene;
    GameObject m_SceneName;
    

    // Use this for initialization
    void Start () {
        m_SceneName = GameObject.Find("GameOver");
        m_retryScene = m_SceneName.GetComponent<SceneName>().m_retryScene;
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void PushTitle()
    {
        m_canvas.GetComponent<FadeManager>().SceneChange(m_titleName);
    }

    public void PushRetry()
    {
        if (m_retryScene == m_stage1)
        {
            m_canvas.GetComponent<FadeManager>().SceneChange(m_retryScene);
            m_eventSystem.SetActive(false);
        }

        if (m_retryScene == m_stage2)
        {
            m_canvas.GetComponent<FadeManager>().SceneChange(m_stage2);
            m_eventSystem.SetActive(false);
        }

        if (m_retryScene == m_stage3)
        {
            m_canvas.GetComponent<FadeManager>().SceneChange(m_stage3);
            m_eventSystem.SetActive(false);
        }

        if (m_retryScene == m_stage4)
        {
            m_canvas.GetComponent<FadeManager>().SceneChange(m_stage4);
            m_eventSystem.SetActive(false);
        }

        if (m_retryScene == m_stage5)
        {
            m_canvas.GetComponent<FadeManager>().SceneChange(m_stage5);
            m_eventSystem.SetActive(false);
        }
    }
}
