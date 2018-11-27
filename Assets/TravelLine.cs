using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelLine : MonoBehaviour {

    public Transform origin;
    public Transform sunset;
    public float journeyTime = 5.0F;
    private float startTime;
    void Start()
    {
        startTime = Time.time;
    }
    void Update()
    {
        Vector3 center = (origin.position + sunset.position) * 0.5F;
        center -= new Vector3(0, 0.5f, 0);
        Vector3 riseRelCenter = origin.position - center;
        Vector3 setRelCenter = sunset.position - center;
        float fracComplete = (Time.time - startTime) / journeyTime;
        transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        transform.position += center;
    }
}
