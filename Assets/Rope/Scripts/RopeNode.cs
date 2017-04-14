using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SpringJoint))]
public class RopeNode : MonoBehaviour
{
    public RopeNode parent;

    public RopeNode child;

    public new Rigidbody      rigidbody      { get; private set; }
    public     SpringJoint    springJoint    { get; private set; }
    public     SphereCollider sphereCollider { get; private set; }

    ///<summary>ロープの先頭か?</summary>
    public bool isRoot => parent == null;

    ///<summary>ワールド座標</summary>
    public Vector3 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    ///<summary>初期化</summary>
    void Awake()
    {
        rigidbody      = GetComponent<Rigidbody>();
        springJoint    = GetComponent<SpringJoint>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    ///<summary>ルートノードを返す</summary>
    public RopeNode GetRootNode()
    {
        RopeNode node = parent;
        while (node != null)
        {
#if UNITY_EDITOR
            if (node == this)
            {
                Debug.LogError("Infinity Loop!");
                throw null;
            }
#endif
            node = node.parent;
        }

        return node;
    }

    ///<summary>子の設定</summary>
    public void SetChild(RopeNode childNode)
    {
        childNode.parent = this;
        child = childNode;

        childNode.springJoint.connectedBody = rigidbody;
    }

    ///<summary>親の設定</summary>
    public void SetParent(RopeNode parentNode)
    {
        parentNode.child = this;
        parent = parentNode;

        springJoint.connectedBody = parent.rigidbody;
    }
}
