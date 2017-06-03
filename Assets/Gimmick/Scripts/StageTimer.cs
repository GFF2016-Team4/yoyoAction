using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class StageTimer : MonoBehaviour
{
    public float timer;
    public UnityEvent timeOverEvent;
    public bool isPlay = true;

    [SerializeField]
    private bool initOnPlayerPrefsLoad = false;

    float startTime;

    public float StageClearTime
    {
        get { return startTime - timer; }
    }

    void Reset()
    {
        //ここにUnityEventの初期化処理を書いておく 以下は例
        //GameObject gameManager = GameObject.Find("GameManager");
        //GameManager manager = gameManager.GetComponent<GameManager>();
        //timeOverEvent.AddListener(manager.TimeOver());
    }

    void Awake()
    {
        startTime = timer;
    }

    IEnumerator Start()
    {
        if (initOnPlayerPrefsLoad) { LoadTimer(); }

        while (true)
        {
            UpdateTimer();
            if (timer <= 0.0f)
            {
                //コールバック
                timeOverEvent.Invoke();
                yield return null;
            }
            yield return null;
        }
    }

    void UpdateTimer()
    {
        //ポーズ時にタイマーを動かさない
        if (Time.timeScale == 0 || !isPlay) { return; }

        timer -= Time.deltaTime;

        //制限
        timer = Mathf.Max(timer, 0.0f);
    }
    
    public void SaveTimer()
    {
        PlayerPrefs.SetFloat("start_time", startTime);
        PlayerPrefs.SetFloat("timer",      timer);
    }

    public void LoadTimer()
    {
        const float maxTime = 99*60 + 99;
        timer     = PlayerPrefs.GetFloat("timer",      maxTime);
        startTime = PlayerPrefs.GetFloat("start_time", maxTime);
    }
}
