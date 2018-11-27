using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sw33.arkit;
using UnityEngine.XR.iOS;
using UnityEngine.Events;
using System.Linq;

public class PlaceOnClick : MonoBehaviour {
	public bool isPlacing = true;
	public Transform placingTransform;
	public GameObject placingHelpers;

	public UnityEvent onObjectPlaced;

	// Use this for initialization
	void Start () {
		ARKitInputManager.Instance.onPlaneHit.AddListener(onPlaneHit);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		

	void onPlaneHit(List<ARHitTestResult> hitresults ){
		if (!isPlacing)
			return;
		
		foreach (ARHitTestResult arhr in hitresults) {

			placingTransform.position = UnityARMatrixOps.GetPosition (arhr.worldTransform);
			placingTransform.rotation = UnityARMatrixOps.GetRotation (arhr.worldTransform);
			
			UnityARUserAnchorComponent component = placingTransform.gameObject.AddComponent<UnityARUserAnchorComponent>();
			isPlacing = false;
			placingHelpers.SetActive (false);

			if (onObjectPlaced != null)
				onObjectPlaced.Invoke ();

			UnityARUtility.InitializePlanePrefab (null);

			List<ArPlaneID> arps = FindObjectsOfType<ArPlaneID> ().ToList();

			foreach (ArPlaneID ai in arps) {
				ai.StopPlane ();
			}
			
			return;

		}
	}
		

	void removeAnchor(){
		placingHelpers.SetActive (true);
		UnityARSessionNativeInterface.GetARSessionNativeInterface().RemoveUserAnchor(placingTransform.GetComponent<UnityARUserAnchorComponent>().AnchorId);
		isPlacing = true;
	}
}
