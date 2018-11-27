using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sw33.Utilities;
using UnityEngine.Events;
using UnityEngine.XR.iOS;
using sw33.UnityEvents;
using UnityEngine.EventSystems;

namespace sw33.arkit {
	public class ARKitInputManager : Singleton<ARKitInputManager> {
		[Header("Unity Events")]
		public ArPlaneHit onPlaneHit;

		[Space(20)]
		[Header("Configuration")]
		public bool useUnityEvents = true;


		PointerEventData pointerEventData = null;

		// Update is called once per frame
		void Update () {
			HandleClick ();
		}

		void HandleClick(){
			if (Input.touchCount > 0)
			{
				var touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
				{
					var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
					ARPoint point = new ARPoint {
						x = screenPosition.x,
						y = screenPosition.y
					};

					// prioritize reults types
					ARHitTestResultType[] resultTypes = {
						ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
						// if you want to use infinite planes use this:
						//ARHitTestResultType.ARHitTestResultTypeExistingPlane,
						ARHitTestResultType.ARHitTestResultTypeHorizontalPlane, 
						ARHitTestResultType.ARHitTestResultTypeFeaturePoint
					}; 

					foreach (ARHitTestResultType resultType in resultTypes)
					{  
						List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultType);
						if (hitResults.Count > 0) {
							if(onPlaneHit != null) onPlaneHit.Invoke(hitResults);
						}
					}

					RaycastHit hit;
					Touch[] touches = Input.touches;
					foreach (Touch  t  in touches){
						Ray ray = Camera.main.ScreenPointToRay (new Vector3(touch.position.x, touch.position.y,0));

						if (Physics.Raycast (ray,out hit,Mathf.Infinity)){
							if (hit.collider != null && hit.collider.gameObject != null) {
								PointerEventData ped = new PointerEventData(EventSystem.current);
								ExecuteEvents.Execute (hit.collider.gameObject, ped, ExecuteEvents.pointerClickHandler);
								//ExecuteEvents.Execute (hit.collider.gameObject, EventSystem.current, ExecuteEvents.pointerClickHandler);
							}
						}
					}
				}
			}

		}
							
	}
}
