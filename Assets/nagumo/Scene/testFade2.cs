using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testFade2 : MonoBehaviour {

    public GameObject canvas;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Space))
        canvas.GetComponent<FadeManager>().SceneChange("StageSelect");
    }
}
