using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
public class StageRank : MonoBehaviour
{
    [SerializeField]
    StageTimer timerComponent;

    Text rankDrawer;

    void Awake()
    {
        rankDrawer = GetComponent<Text>();
    }

    IEnumerator Start()
    {
        //ロードに時間がかかる可能性があるため
        yield return null;

        float  timer = timerComponent.StageClearTime;
        string rank = LoadRank(timer);
        
        rankDrawer.text = "Rank : " + rank ;
    }

    string LoadRank(float timer)
    {
        Debug.Log(timer);

        float time_S = PlayerPrefs.GetFloat("SRank", 60.0f);
        float time_A = PlayerPrefs.GetFloat("ARank", 90.0f);
        float time_B = PlayerPrefs.GetFloat("BRank", 120.0f);

        if (timer <= time_S) return "S";
        if (timer <= time_A) return "A";
        if (timer <= time_B) return "B";
        return "C";
    }
}
