using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneName : MonoBehaviour {

    [System.NonSerialized]
    public string m_retryScene;
    [Header("それぞれのステージ名")]
    public string m_stage1;
    public string m_stage2;
    public string m_stage3;
    public string m_stage4;
    public string m_stage5;

    string m_sceneName;

    public static SceneName Instance
    {
        get;
        private set;
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //現在のシーンの名前を取得
        //m_retryScene = SceneManager.GetActiveScene().name;

        //GameObjectの保存(Scene遷移時も残る)
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start () {
        
        
    }
	
	// Update is called once per frame
	void Update () {
        m_sceneName = SceneManager.GetActiveScene().name;
        if (m_sceneName == m_stage1)
            m_retryScene = m_stage1;
        if (m_sceneName == m_stage2)
            m_retryScene = m_stage2;
        if (m_sceneName == m_stage3)
            m_retryScene = m_stage3;
        if (m_sceneName == m_stage4)
            m_retryScene = m_stage4;
        if (m_sceneName == m_stage5)
            m_retryScene = m_stage5;
    }
}
