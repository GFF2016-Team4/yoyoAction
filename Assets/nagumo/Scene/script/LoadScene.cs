using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour {
    public float speed;
    public List<Graphic> fadeUIList;

    void SetAlphaUI(float t)
    {
        foreach(var ui in fadeUIList)
        {
            Color alpha = ui.color;
            alpha.a = t;
            ui.color = alpha;
        }
    }

	// Use this for initialization
	IEnumerator Start () {
		for(float t = 0; t < 1.0f; t += Time.deltaTime * speed)
        {
            SetAlphaUI(t);
            yield return null;
        }
        SetAlphaUI(1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
