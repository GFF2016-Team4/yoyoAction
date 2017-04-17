using UnityEngine;
using System;
using System.Collections.Generic;

public class Rail : MonoBehaviour
{
    public Vector3[] points;
    public string    railName = "Rail";

    public GameObject createObject;

    void Reset()
    {
        points = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f)
        };
    }

    public void AddPoint()
    {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 1);
        point.x += 1f;
        points[points.Length - 1] = point;
    }

    public void RemovePoint(int index)
    {
        List<Vector3> list = new List<Vector3>(points);
        list.RemoveAt(index);
        points = list.ToArray();
    }

    public void Generate()
    {
        if (createObject == null)
        {
            Debug.LogError("エラー : プレハブが設定されていません");
            return;
        }

        GameObject railParent = new GameObject();
        railParent.name = railName;

        for (int i = 0; i < points.Length-1; i++)
        {
            Vector3 p1 = points[i + 0];
            Vector3 p2 = points[i + 1];

            //中点
            Vector3    midPoint = CalcMidPoint(p1, p2);
            float      distance = Vector3.Distance(p1, p2);
            Quaternion quat     = Quaternion.LookRotation(CalcDirection(p1, p2));

            //生成
            GameObject obj = Instantiate(createObject);

            //初期化
            obj.transform.position = midPoint;
            obj.transform.rotation = quat;

            Vector3 scale = obj.transform.localScale;
            scale.z  = distance;
            
            obj.transform.localScale = scale;

            obj.transform.parent = railParent.transform;
        }
    }
    
    Vector3 CalcMidPoint(Vector3 vec1, Vector3 vec2)
    {
        return (vec1 + vec2) / 2.0f;
    }

    Vector3 CalcDirection(Vector3 vec1, Vector3 vec2)
    {
        return vec2 - vec1;
    }
}
