using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour {

    SpriteRenderer m_renderer;

    public GameObject m_canvas;
    public GameObject m_spriteRenderer;
    [Tooltip("画像１")]
    public Sprite m_image1;
    [Tooltip("画像２")]
    public Sprite m_image2;


    [Header("ステージ１のシーン名")]
    public string m_stage1;

    [Header("ステージ２のシーン名")]
    public string m_stage2;

    [Header("ステージ３のシーン名")]
    public string m_stage3;

    [Header("ステージ４のシーン名")]
    public string m_stage4;

    [Header("ステージ５のシーン名")]
    public string m_stage5;

    [Header("タイトルのシーン名")]
    public string m_title;

    // Use this for initialization
    void Start () {
        m_renderer = m_spriteRenderer.GetComponent<SpriteRenderer>();
        m_renderer.sprite = m_image1;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.T))
            m_renderer.sprite = m_image1;
        if (Input.GetKeyDown(KeyCode.Y))
            m_renderer.sprite = m_image2;
    }

    public void PushStage1()
    {
        m_canvas.GetComponent<FadeManager>().SceneChange(m_stage1);
        this.gameObject.SetActive(false);
    }
    public void PushStage2()
    {
        m_canvas.GetComponent<FadeManager>().SceneChange(m_stage2);
        this.gameObject.SetActive(false);
    }
    public void PushStage3()
    {
        m_canvas.GetComponent<FadeManager>().SceneChange(m_stage3);
        this.gameObject.SetActive(false);
    }
    public void PushStage4()
    {
        m_canvas.GetComponent<FadeManager>().SceneChange(m_stage4);
        this.gameObject.SetActive(false);
    }
    public void PushStage5()
    {
        m_canvas.GetComponent<FadeManager>().SceneChange(m_stage5);
        this.gameObject.SetActive(false);
    }
    public void PushTitle()
    {
        m_canvas.GetComponent<FadeManager>().SceneChange(m_title);
        this.gameObject.SetActive(false);
    }
}
