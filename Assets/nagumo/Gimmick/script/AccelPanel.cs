using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelPanel : MonoBehaviour {
    public GameObject player;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

	}

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            player.GetComponent<Player>().AccelAdd(10);
        }
    }
}
