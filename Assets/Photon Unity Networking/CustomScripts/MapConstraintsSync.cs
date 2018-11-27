using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (PhotonView))]
public class MapConstraintsSync : Photon.MonoBehaviour {

	bool registred = false;
	public CitiesLocator cl;

	void Start(){
		Application.LoadLevelAdditive("ARKitMapbox");
	}

	void Update()
	{
        if (cl == null)
        {
            cl = FindObjectOfType<CitiesLocator>();
        }
		else if (!registred) {
			registred = true;
			Debug.Log ("resgistred");
			cl.onCityClickedEvent.AddListener (CityClicked);
            cl.onResetEvent.AddListener(OnReset);
		}
		
		PhotonView photonView = this.photonView;

		if(PhotonNetwork.connected)  photonView.RPC("Test", PhotonTargets.All);
	}


	public void CityClicked(string  cityIata){
		if(PhotonNetwork.connected)  photonView.RPC("CityClickedNet", PhotonTargets.Others, cityIata);	
	}

    public void OnReset(){
        if(PhotonNetwork.connected)  photonView.RPC("ResetSc", PhotonTargets.Others);  
    }

	[PunRPC]
	public void CityClickedNet(string cityIata){
		cl.cityMarkerClicked (cityIata);
	}

	[PunRPC]
	void Test()
	{
		if (cl == null)
			return;
		
	/*	photonView.RPC("OnChangeDeptCity", PhotonTargets.All, cl.origCity.IataCode);
		photonView.RPC("OnChengArrivalCity", PhotonTargets.All, cl.endCity.IataCode);

		photonView.RPC("OnChengDepAirport", PhotonTargets.All, cl.origenAirport.Id);
		photonView.RPC("OnChengArrivalAirport", PhotonTargets.All, cl.endAirport.Id);*/
		
	}

    [PunRPC]
    public void ResetSc()
    {
        cl.RemoteReset();
    }
		

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
		}

		else
		{
		}
	}
}
