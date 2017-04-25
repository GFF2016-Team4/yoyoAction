using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Lift : MonoBehaviour
{
    [Header("リフトの始点はオブジェクトの位置で決まる")]
    [Space(10)]
    [Header("リフトの始点")]
    public Vector3 startPoint;

    [Header("リフトの終点")]
    public Vector3 endPoint;

    [Header("停止時間")]
    public float   stayTime;

    [Header("移動する時間(秒)")]
    public float   moveSecond;

    void OnValidate()
    {
        transform.position = startPoint;
        
#pragma warning disable CS0618
        MoveRestart();
#pragma warning restore CS0618
    }

    void Start()
    {
        StartCoroutine(MoveLift());
    }

    /// <summary> リフトを動かします</summary>
    IEnumerator MoveLift()
    {
        //往復を繰り返す
        while (true)
        {
            yield return Move(startPoint, endPoint);
            yield return new WaitForSeconds(stayTime);

            yield return Move(endPoint, startPoint);
            yield return new WaitForSeconds(stayTime);
        }
    }

    /// <summary>リフトをstartPointからもう一度 動かす</summary>
    [Obsolete("Unity Editor以外では動きません")]
    public void MoveRestart()
    {
#if UNITY_EDITOR
        //停止中に動かないようにする
        if (EditorApplication.isPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(MoveLift());
        }
#endif
    }

    /// <summary>リフトをstratからendまで動かす</summary>
    IEnumerator Move(Vector3 start, Vector3 end)
    {
        transform.position = start;
        
        for (float time = 0.0f; time < moveSecond; time += Time.deltaTime)
        {
            //0~1の範囲に変換
            float t = time / moveSecond;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end;
    }
}
