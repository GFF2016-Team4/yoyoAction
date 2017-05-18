using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
public class StageTimeView : MonoBehaviour
{
    [SerializeField]
    StageTimer timerComponent;

    [SerializeField, Header("数字の前に表示させる文字")]
    public string textTimeString;

    [Header("分と秒のフォーマット 分からなければ聞いてください")]
    public string minFormat = "0";
    public string secFormat = "00.000";

    Text timerDrawer;

    void Awake()
    {
        timerDrawer = GetComponent<Text>();
    }

    void LateUpdate()
    {
        int   min     = (int)Mathf.Floor(timerComponent.timer / 60.0f);
        float secound = timerComponent.timer - (min * 60);

        string time = string.Format("{0:"+minFormat+"}:{1:"+secFormat+"}", min, secound);

        timerDrawer.text = textTimeString + time;
    }
}
