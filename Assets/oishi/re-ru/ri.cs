using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ri : MonoBehaviour
{
    public GameObject prefab;

    Vector3 sakuseiPos;
    bool a = false;

    // Use this for initialization
    void Start()
    {
        sakuseiPos = GameObject.Find("go-ru").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            prefab = Instantiate(prefab, sakuseiPos, Quaternion.identity);

            a = true;
        }


        if (a == true)
        {
            Vector3 douki = new Vector3(prefab.transform.position.x, 0, prefab.transform.position.z);
            gameObject.transform.position = douki;
        }
    }
}
