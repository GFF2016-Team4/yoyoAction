using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Lift : MonoBehaviour
{
    [Header("リフトの始点はオブジェクトの位置で決まる")]
    [Space(10)]
    [Header("リフトの始点")]
    public Vector3 startPoint;

    [Header("リフトの終点")]
    public Vector3 endPoint;

    public float   stayTime;
    public float   moveSecond;

    void OnValidate()
    {
        transform.position = startPoint;

#if UNITY_EDITOR
        MoveRestart();
#endif
    }

    void Start()
    {
#if UNITY_EDITOR
        //ExecuteInEditModeを付けていると停止中でも勝手に動いてしまうので、停止中に動かないようにする
        //(ExecuteInEditModeはオブジェクトの位置とstartPointを同期させるためにつけている)
        if (!EditorApplication.isPlaying) return;
#endif
        startPoint = transform.position;
        StartCoroutine(MoveLift());
    }

    /// <summary> リフトを動かします</summary>
    IEnumerator MoveLift()
    {
        while (true)
        {
            yield return Move(startPoint, endPoint);
            yield return new WaitForSeconds(stayTime);

            yield return Move(endPoint, startPoint);
            yield return new WaitForSeconds(stayTime);
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        //オブジェクトの位置を変えた時 同期しないことを防ぐため
        if (!EditorApplication.isPlaying) { startPoint = transform.position;}
    }
#endif

    /// <summary>リフトをstartPointからもう一度 動かす</summary>
    public void MoveRestart()
    {
        StopAllCoroutines();
        StartCoroutine(MoveLift());
    }

    /// <summary>リフト同期用</summary>
    Coroutine WaitForMoveLift(Vector3 start, Vector3 end)
    {
        return StartCoroutine(Move(start, end));
    }

    /// <summary>リフトをstratからendまで動かす</summary>
    IEnumerator Move(Vector3 start, Vector3 end)
    {
        transform.position = start;

        for (float time = 0.0f; time < moveSecond; time += Time.deltaTime)
        {
            float t = time / moveSecond;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end;
    }
}
