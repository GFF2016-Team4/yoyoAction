using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class StageTimer : MonoBehaviour
{
    public float timer;

    public UnityEvent timeOverEvent;

    void Reset()
    {
        //ここにUnityEventの初期化処理を書いておく 以下は例
        //GameObject gameManager = GameObject.Find("GameManager");
        //GameManager manager = gameManager.GetComponent<GameManager>();
        //timeOverEvent.AddListener(manager.TimeOver());
    }

    IEnumerator Start()
    {
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
        if (Time.timeScale == 0) { return; }

        timer -= Time.deltaTime;

        //制限
        timer = Mathf.Max(timer, 0.0f);
    }
}
