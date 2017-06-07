using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageSelect : MonoBehaviour {

    SpriteRenderer m_renderer;
    RectTransform m_rectTrans;

    public GameObject m_canvas;
    public GameObject m_spriteRenderer;

    [Header("ステージ１")]
    [Tooltip("ステージ１シーン名")]
    public string m_stage1;
    [Tooltip("ステージ１画像")]
    public Sprite m_image1;
    [Tooltip("ステージ１ボタン")]
    public GameObject m_stage1Button;

    [Header("ステージ２")]
    [Tooltip("ステージ２シーン名")]
    public string m_stage2;
    [Tooltip("ステージ２画像")]
    public Sprite m_image2;
    [Tooltip("ステージ２ボタン")]
    public GameObject m_stage2Button;

    [Header("ステージ３")]
    [Tooltip("ステージ３シーン名")]
    public string m_stage3;
    [Tooltip("ステージ３画像")]
    public Sprite m_image3;
    [Tooltip("ステージ３ボタン")]
    public GameObject m_stage3Button;

    [Header("ステージ４")]
    [Tooltip("ステージ４シーン名")]
    public string m_stage4;
    [Tooltip("ステージ４画像")]
    public Sprite m_image4;
    [Tooltip("ステージ４ボタン")]
    public GameObject m_stage4Button;

    [Header("ステージ５")]
    [Tooltip("ステージ５シーン名")]
    public string m_stage5;
    [Tooltip("ステージ５画像")]
    public Sprite m_image5;
    [Tooltip("ステージ５ボタン")]
    public GameObject m_stage5Button;

    [Header("タイトルのシーン名")]
    public string m_title;

    void Awake()
    {
        m_rectTrans = GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start () {
        m_renderer = m_spriteRenderer.GetComponent<SpriteRenderer>();
        m_renderer.sprite = m_image1;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        //EventSystem に今選択中のオブジェクトを教えてもらう
        GameObject selectedObject =
            EventSystem.current.currentSelectedGameObject;

        //m_stage1Buttonが選択中なら
        if(selectedObject == m_stage1Button)
        {
            m_renderer.sprite = m_image1;
        }
        //m_stage2Buttonが選択中なら
        if (selectedObject == m_stage2Button)
        {
            m_renderer.sprite = m_image2;
        }
        //m_stage3Buttonが選択中なら
        if (selectedObject == m_stage3Button)
        {
            m_renderer.sprite = m_image3;
        }
        //m_stage4Buttonが選択中なら
        if (selectedObject == m_stage4Button)
        {
            m_renderer.sprite = m_image4;
        }
        //m_stage5Buttonが選択中なら
        if (selectedObject == m_stage5Button)
        {
            m_renderer.sprite = m_image5;
        }

        //選択中のオブジェクトの場所にカーソルを表示
        //m_rectTrans.anchoredPosition =
        //    selectedObject.GetComponent<RectTransform>().anchoredPosition;
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
