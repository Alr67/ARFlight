using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AROnClickTest : MonoBehaviour, IPointerClickHandler {
	public UnityEvent<GameObject> onClickUnityEvent;

	public Material m1;
	public Material m2;

	bool m1On = true;
	// Use this for initialization
	void Start () {
		GetComponent<Renderer> ().material = m1;
		m1On = true;
	}

	// Update is called once per frame
	void Update () {
		
	}

	[ContextMenu ("Execute on click")]

	public void OnPointerClick(PointerEventData ed){
		if (onClickUnityEvent != null)
			onClickUnityEvent.Invoke (gameObject);

		if(m1On) GetComponent<Renderer>().material = m2;
		else GetComponent<Renderer>().material = m1;

		m1On = !m1On;

	}
}
