using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_controler : MonoBehaviour {

    public GameObject player;

    private Vector3 offset; // to hold the ofset between camera and ball, private, da im script definiert

	// Use this for initialization
	void Start () {
        offset = transform.position - player.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = player.transform.position + offset;
	}
}
