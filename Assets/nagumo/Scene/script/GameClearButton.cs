using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameClearButton : MonoBehaviour {
    [Header("フェード用Canvas")]
    public GameObject m_canvas;
    public GameObject m_eventSystem;

    [Header("タイトルシーン名")]
    public string m_title;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
    }
    public void PushTitle()
    {
        m_canvas.GetComponent<FadeManager>().SceneChange(m_title);
        m_eventSystem.SetActive(false);
    }
}
