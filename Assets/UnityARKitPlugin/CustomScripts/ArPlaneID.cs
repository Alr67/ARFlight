using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArPlaneID : MonoBehaviour {
	public MeshCollider mc;
	public BoxCollider bc;
	public MeshRenderer mr;

	public void StopPlane(){
		mc.enabled = false;
		bc.enabled = false;
		mr.enabled = false;
	}
}
