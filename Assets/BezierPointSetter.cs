using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierPointSetter : MonoBehaviour {

    public GameObject p1, p2, p3, p4;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float SetPoints(Vector3 departure, Vector3 arrival) {
        
        p1.transform.position = departure;
        p4.transform.position = arrival;

        var finalX = (arrival.x - departure.x) / 2;
        var finalZ = (arrival.z - departure.z) / 2;
        Vector3 tempPos = new Vector3(departure.x + finalX, departure.y + Mathf.Abs(finalX), departure.z + finalZ);

        p2.transform.position = tempPos;
        p3.transform.position = tempPos;

        return Vector3.Distance(departure, arrival);
    }

    public void ClearPoints() {
        var tempVec = new Vector3(0, 0, 0);
        p1.transform.position = tempVec;
        p2.transform.position = tempVec;
        p3.transform.position = tempVec;
        p4.transform.position = tempVec;
    }
}
