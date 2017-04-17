using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoyoCollision : MonoBehaviour {

    [SerializeField, Header("回転速度")]
    public float m_RotateSpeed = 180.0f;

    private Transform m_YoyoObject;
    private Rigidbody m_Rigidbody;

    // Use this for initialization
    void Start () {
        //m_YoyoObject = this.transform.parent.GetComponent<Yoyo>().transform;
        //m_Rigidbody = this.transform.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        //ヨーヨーの自転
        YoyoRotate(m_RotateSpeed);
    }

    //ヨーヨーの自転メソッド
    void YoyoRotate(float rotateSpeed)
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ball")
        {
            //Debug.Log("Enter");
            //m_YoyoObject.GetComponent<Yoyo>().StopAllCoroutines();
            //m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

}
