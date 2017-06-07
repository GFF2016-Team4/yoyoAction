using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorSample : MonoBehaviour {

    RectTransform m_rectTrans;

    void Awake()
    {
        m_rectTrans = GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //EventSystem に今選択中のオブジェクトを教えてもらう
        GameObject selectedObject =
            EventSystem.current.currentSelectedGameObject;
        //選択中のオブジェクトの場所にカーソルを表示
        m_rectTrans.anchoredPosition =
            selectedObject.GetComponent<RectTransform>().anchoredPosition;
    }
}
