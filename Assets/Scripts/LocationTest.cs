using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTest : MonoBehaviour {


    public float latitud = 41.7278f;
    public float longitud = 0.5358f;
    public Airport[] aeroports;

    // Use this for initialization
    void Start () {
        BasicMap map = gameObject.GetComponent<BasicMap>();
        if (map == null) Debug.LogWarning("No he trobat el mapa");
        foreach (Airport aer in aeroports) {
            var sf = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var position = VectorExtensions.AsUnityPosition(new Vector2(latitud, longitud), map.CenterMercator, map.WorldRelativeScale);
            sf.transform.position = position;
        }
        
        
    }
	
	// Update is called once per frame
	void Update ()
    {
       
    }

}
