using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameTitleButton : MonoBehaviour {

    public GameObject m_canvas;
    public GameObject m_eventSystem;

    [Header("ゲーム選択シーン名")]
    public string m_selectScene;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PushSelect()
    {
        m_canvas.GetComponent<FadeManager>().SceneChange(m_selectScene);
        m_eventSystem.SetActive(false);
    }
    public void GameEnd()
    {
        Application.Quit();
    }
}
