using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOnStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2 - Camera.main.transform.up * 2*100;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
