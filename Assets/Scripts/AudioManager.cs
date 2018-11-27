using sw33.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {

    public AudioClip selectDeperature;
    public AudioClip selectDestination;
    public AudioClip noRoute;
    public AudioClip lookingBestPrice;
    public AudioClip flightOkey;
	public AudioSource audioSource ;


    public void PlaySelectDepartureAudio()
    {
        audioSource.clip = selectDeperature;
        audioSource.Play();
    }
	public void PlaySelectDestinationAudio()
    {
        audioSource.clip = selectDestination;
        audioSource.Play();
    }
	public void PlayNoRouteAudio()
    {
        audioSource.clip = noRoute;
        audioSource.Play();
    }
	public void PlayLookingBestPriceAudio()
    {
        audioSource.clip = lookingBestPrice;
        audioSource.Play();

    }
	public void PlayFlightOkeyAudio()
    {
        audioSource.clip = flightOkey;
        audioSource.Play();
    }
}

