using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testFade : MonoBehaviour {

    public GameObject canvas;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //GetComponent<FadeManager>().FadeIn();
            //GetComponent<FadeManager>().FadeOut();
            canvas.GetComponent<FadeManager>().SceneChange("Scene2");
        }
    }
}
