using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelPanel : MonoBehaviour {
    GameObject player;
    public float accel = 100f;

	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("Player");
    }

	// Update is called once per frame
	void Update () {

	}

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            player.GetComponent<Player>().Accel(accel);
            //player.GetComponent<Player>().speed *= 5;
        }
    }
}
