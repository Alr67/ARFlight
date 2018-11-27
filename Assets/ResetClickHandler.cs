using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetClickHandler : MonoBehaviour, IPointerClickHandler {

    public GameObject map;

    public void OnPointerClick(PointerEventData eventData)
    {
        map.GetComponent<CitiesLocator>().RequestedReset();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [ContextMenu("EditorOnClick")]
    public void EditorOnClick()
    {
        OnPointerClick(null);
    }
}
