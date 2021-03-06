﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RopeSimulate : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField, Tooltip("ロープの構造体")]
    Rope        rope;

    [SerializeField]
    float       checkDistance;

    [SerializeField]
    float       takeupSpeed;

    [SerializeField]
    LayerMask   ignoreMask;
#pragma warning restore 0649

    // ロープの描画
    private ListLineDraw listLineDraw;

    private bool isSimulationEnd = false;

    private const float ignoreDistance = 0.2f;

    //使いまわし
    private Ray ray;

    /// <summary>シミュレート中か</summary>
    public bool isSimulate => !rope.tailKinematic;

    
    public Vector3 originPosition
    {
        get { return rope.rigOrigin.position;  }
        set { rope.rigOrigin.position = value; }
    }

    public Vector3 tailPosition
    {
        get { return rope.tail.position; }
        set { rope.tailPosition = value; }
    }

    void Awake()
    {
        //初期化
        listLineDraw = GetComponent<ListLineDraw>();
    }

    IEnumerator Start()
    {
        rope.ReCalcDistance();
        
        //loop
        while (true)
        {
            RopeUpdate();
            yield return null;
        }
    }

#if UNITY_EDITOR
    [System.Obsolete("InitPosition(Vector3 origin, Vector3 tail)を使用してください")]
    public void Initialize(Vector3 origin, Vector3 tail)
    {
        InitPosition(origin, tail);
    }
#endif

    public void InitPosition(Vector3 origin, Vector3 tail)
    {
        bool simulate = isSimulate;
        if (simulate) SimulationStop();

        rope.originPosition = origin;
        rope.tailPosition   = tail;
        rope.ResetDistance();
        rope.ReCalcDistance();

        if (simulate) SimulationStart();
    }

    void RopeUpdate()
    {
        if(IsRemoveOrigin())
        {
            RemoveOrigin();
            return;
        }

        //ロープのどこかが当たった場合
        Vector3 direction = rope.directionNormalized;
        direction *= ignoreDistance;

        if (Physics.Linecast(rope.tailPosition, rope.originPosition-direction, out RaycastHit hitInfo, ignoreMask))
        {
            ChangeNewRigOrigin(hitInfo.point);
        }
    }

    void ChangeNewRigOrigin(Vector3 position)
    {
        RopeNode newOrigin = rope.AddRigOrigin(position);
        newOrigin.transform.parent = transform;
        listLineDraw.Insert(listLineDraw.count - 1, newOrigin.transform);

        newOrigin.name = "origin" + (transform.childCount - 1);
    }

    bool IsRemoveOrigin()
    {
        RopeNode rigOrigin  = rope.rigOrigin;
        RopeNode prevOrigin = rope.prevRigOrigin;

        //ルートの場合は引っかかっていることがない
        if(rigOrigin.isRoot) return false;
        
        //Rangeが大きければ判定しない
        if(!CheckRange(prevOrigin.position, rigOrigin.position)) return false;

        //障害物に当たってない -> 削除しないと不自然
        ray.origin =  rope.tailPosition;
        ray.direction = -rope.tailPosition + prevOrigin.position;

        float maxDistance = Vector3.Distance(rope.tailPosition, prevOrigin.position);
        maxDistance -= ignoreDistance;

        if(Physics.Raycast(ray, maxDistance, ignoreMask)) return false;

        return true;
    }

    void RemoveOrigin()
    {
        //引っかかりを取る
        listLineDraw.RemoveDrawList(rope.rigOrigin.transform);
        rope.RemoveLastRigOrigin();
    }

    bool CheckRange(Vector3 linePoint1, Vector3 linePoint2)
    {
        //線分の始点と方向を出す
        Vector3 origin    = linePoint1;
        Vector3 direction = Vector3.Normalize(linePoint2 - linePoint1);

        //線分と点の距離を求める
        Vector3 v = tailPosition - origin;
        float   d = Vector3.Dot(v, direction);
        
        float distance = Vector3.Magnitude(-v + direction * d);

        return distance <= checkDistance;
    }

    public void SimulationStart()
    {
        rope.tailKinematic = false;
        rope.ResetDistance();
        rope.ReCalcDistance();
    }

    public void SimulationStop()
    {
        rope.tailKinematic = true;
    }

    [System.Obsolete("エラーになります")]
    public void SimulationEnd(Transform sync)
    {
        Debug.Log("is end?");

        if (isSimulationEnd) return;
        isSimulationEnd = true;

        Debug.Log("end");

        StopAllCoroutines();
        SimulationStop();

        //ルートノードとロープの末尾のノードの２つだけになるようにする
        RopeNode node = rope.rigOrigin;
        while(!node.isRoot)
        {
            RopeNode parent = node.parent;
            
            listLineDraw.RemoveDrawList(node.transform);
            Destroy(node.gameObject);

            node = parent;
        }
        rope.SetRigOrigin(node);

        //自由落下
        rope.originKinematic = false;
        Destroy(rope.rigOrigin.springJoint);
        //Destroy(rope.rigOrigin.GetComponent<SyncObject>());

        rope.rigOrigin.transform.rotation = Quaternion.LookRotation(-rope.direction);

        StartCoroutine(TakeUp(sync));
    }

    private IEnumerator TakeUp(Transform sync)
    {
        //巻き取りの開始
        Vector3 startPos = rope.originPosition;

        float distance = Vector3.Distance(rope.tailPosition, rope.originPosition);
        if (distance == 0.0f)
        {
            Debug.Log("return");
            yield break;
        }

        while (Vector3.Distance(rope.tailPosition, rope.originPosition) > 0.3f)
        {
            rope.tailPosition = sync.position;

            Vector3 tail = rope.tailPosition;
            Vector3 origin = rope.originPosition;
            rope.originPosition = Vector3.MoveTowards(origin, tail, takeupSpeed);

            yield return null;
        }

        Debug.Log("ok");

        //SoundManager.Instance.StopSE();
        Destroy(gameObject);
    }

    public void AddForce(Vector3 force, ForceMode forceMode)
    {
        rope.tail.rigidbody.AddForce(force, forceMode);
    }
}
