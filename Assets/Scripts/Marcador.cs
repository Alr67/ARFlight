using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[SelectionBase]
public class Marcador : MonoBehaviour, IPointerClickHandler{

    public TextMesh text;
    public Renderer xinxeta;
    public bool changeColor = false;
    public Color noSelectedColor;
    public Color selectedColor;
	public string place;
	public City city;
	public CityClick cityClick = new CityClick();

    void Start()
    {
        if(changeColor)
            xinxeta.material.color = noSelectedColor;
    }

    public void SetText(string title)
    {
        text.text = title;
    }

	[ContextMenu ("EditorOnClick")]
	public void EditorOnClick(){
		OnPointerClick (null);
	}

	public void OnPointerClick(PointerEventData ed){
		if (cityClick != null)
			cityClick.Invoke (gameObject, city);
	}
		


	public void Select(){
		xinxeta.material.color = selectedColor;

	}

	public void Deselect(){
		xinxeta.material.color = noSelectedColor;

	}
}
