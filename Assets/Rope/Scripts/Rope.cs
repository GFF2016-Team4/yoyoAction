using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Rope
{
    [Header("末尾")]
    public RopeNode tail;

    [Header("現在の振り子運動の基準")]
    public RopeNode rigOrigin;

    //プロパティ

    ///<summary>現在の振り子運動の座標と末尾の座標の方向</summary>
    public Vector3  direction           => rigOrigin.position - tail.position;

    ///<summary>現在の振り子運動の座標と末尾の座標の方向(正規化)</summary>
    public Vector3  directionNormalized => direction.normalized;

    ///<summary>現在の振り子運動している部分の長さ</summary>
    public float    length              => direction.magnitude;

    ///<summary>ロープ末尾が物理挙動しているか</summary>
    public bool     isKinematic         => tail.rigidbody.isKinematic;

    ///<summary>前の振り子運動の座標</summary>
    public RopeNode prevRigOrigin       => rigOrigin.parent;
    
    ///<summary>ロープ末尾の座標</summary>
    public Vector3 tailPosition
    {
        get { return tail.position; }
        set
        {
#if UNITY_EDITOR
            if (!tail.rigidbody.isKinematic)
            {
                Debug.Log("物理挙動をしている間は変更できません");
                return;
            }
#endif
            tail.position = value;
        }
    }

    /// <summary>振り子の基準点の座標 </summary>
    public Vector3 originPosition
    {
        get { return rigOrigin.position;  }
        set { rigOrigin.position = value; }
    }

    ///<summary>ロープ末尾の物理挙動</summary>
    public bool tailKinematic
    {
        get { return tail.rigidbody.isKinematic;  }
        set { tail.rigidbody.isKinematic = value; }
    }

    ///<summary>振り子の基点の物理挙動</summary>
    public bool originKinematic
    {
        get { return rigOrigin.rigidbody.isKinematic; }
        set { rigOrigin.rigidbody.isKinematic = value; }
    }

    ///<summary>現在の振り子運動の基点の設定</summary>
    public void SetRigOrigin(RopeNode newOrigin)
    {
        rigOrigin.SetChild(newOrigin);
        tail.SetParent(newOrigin);

        rigOrigin = newOrigin;
    }

    ///<summary>現在の振り子運動の基点の設定</summary>
    public void SetRigOrigin(RopeNode newOrigin, RopeNode parent, RopeNode child, bool reCalcDistance = true)
    {
        if(parent != null) { parent.SetChild (newOrigin); }
        if(child  != null) { child .SetParent(newOrigin); }

        rigOrigin = newOrigin;

        if (reCalcDistance)
        {
            ReCalcDistance();
        }
    }

    ///<summary>振り子運動の基点を新しく追加</summary>
    public RopeNode AddRigOrigin(Vector3 createPoint)
    {
        Transform newRigOrigin = Object.Instantiate(rigOrigin.transform);
        RopeNode  newOrigin    = newRigOrigin.GetComponent<RopeNode>();

        newOrigin.position = createPoint;

        SetRigOrigin(newOrigin);
        ReCalcDistance();

        return newOrigin;
    }

    ///<summary>現在の振り子の基準を消す</summary>
    public void RemoveLastRigOrigin()
    {
        Object.Destroy(rigOrigin.gameObject);

        SetRigOrigin(prevRigOrigin, prevRigOrigin.parent, tail, false);

        if (rigOrigin.childDisntace.HasValue)
        {
            tail.springJoint.minDistance = rigOrigin.childDisntace.Value;
        }
        else
        {
            tail.springJoint.minDistance = length;
        }
    }

    ///<summary>ロープの先頭を返す</summary>
    public RopeNode GetRootRigOrigin()
    {
        return rigOrigin.GetRootNode();
    }

    ///<summary>ロープの長さを計算し直す</summary>
    public void ReCalcDistance()
    {
        tail.springJoint.minDistance = length;
        rigOrigin.childDisntace      = length;
    }

    public void ResetDistance()
    {
        RopeNode node = rigOrigin;
        while (!node.isRoot)
        {
            node.childDisntace = null;
            node = node.parent;
        }
    }
}
