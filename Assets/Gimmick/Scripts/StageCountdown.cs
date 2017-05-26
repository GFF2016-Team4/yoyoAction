using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class StageCountdown : MonoBehaviour
{
    [SerializeField]
    Text text;

    [SerializeField]
    UnityEvent unityEvent;

    WaitForSeconds second = new WaitForSeconds(1.0f);

    IEnumerator Start()
    {
        text.text = "3";
        Debug.Log("3");
        yield return second;

        text.text = "2";
        Debug.Log("2");
        yield return second;

        text.text = "1";
        Debug.Log("1");
        yield return second;

        text.text = "GO";
        Debug.Log("0");
        yield return second;

        unityEvent.Invoke();
    }
}
