using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using Mapbox.Map;
using Mapbox.Utils;
using Mapbox.Unity.MeshGeneration.Data;
using System.Linq;
using UnityEngine.Events;

public class CitiesLocator : MonoBehaviour {

	[Header("Debug vars")]
    private static string apiKey = "ha952718948176934114947694895142";
    private List<City> cities;

	private Dictionary<string, GameObject> markersGo;
	private Dictionary<string,GameObject> airportsGo;

    public RoutesLocator routesLocator;
    public GameObject MarcadorCityPrefab;
	public GameObject MarcadorAirportPrefab;

    public GameObject BaizerLine;
    public GameObject PlanePrefab;
    private GameObject beizerLaser;

    private BasicMap map;

	public CityIDClick onCityClickedEvent = new CityIDClick ();
    public UnityEvent onResetEvent = new UnityEvent();

    [Space(10)]
	[Header("Pool Options")]

	private Queue<GameObject> poolCities;
	public int poolInitSize = 300;
	public float secondsToInitPool = 0;

	public City origCity;
	public City endCity;

	public Airport origenAirport;
	public Airport endAirport;

	public Material selectedMaterial;
	public Material notSelectedMaterial;

	public AppState appState;

	void Awake(){
		poolCities = new Queue<GameObject> ();
		markersGo = new Dictionary<string,GameObject> ();
		airportsGo = new Dictionary<string, GameObject> ();

		for (int i = 0; i < poolInitSize; i++) {
			GameObject go = Instantiate (MarcadorCityPrefab);
			go.name = "Marcador Q";
			go.SetActive (false);
			poolCities.Enqueue (go);
		}
	}
		

	// Use this for initialization
	void Start ()
    {
		appState = AppState.initializing;
        map = GetComponent<BasicMap>();
        GetAndShowCities ();
		Debug.Log ("Audio Started");
      //var thread = new System.Threading.Thread(callSkyApi);
      //thread.Start();
	}

	public void GetAndShowCities(){
		cities = new List<City>();
		StartCoroutine(apiSkyCitysAndAirportsInfo(OnGetInfoFinished));
	}

	// API per els aeroports
	private IEnumerator apiSkyCitysAndAirportsInfo(Action<bool> onFinished)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://partners.api.skyscanner.net/apiservices/geo/v1.0?apiKey="+apiKey);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
			onFinished (false);
        }
        else
        {
            // Show results as text
         //   Debug.Log("result: " + www.downloadHandler.text);
            var json = new JSONObject(www.downloadHandler.text);
            var continentList = json["Continents"];
            foreach (JSONObject continent in continentList.list)
            {
                //    Debug.Log("Continent: " + continent.ToString());
                JSONObject arr = continent["Name"];
                if (arr.str.Equals("Europe"))
                {
                    var countriesList = continent["Countries"];
                    if (countriesList == null) Debug.LogError("No Countries in json");
                    foreach (JSONObject country in countriesList.list)
                    {
                        var citiesList = country["Cities"];
                        if (citiesList == null) Debug.LogError("No Cities in json");
                        foreach (JSONObject cityjson in citiesList.list)
                        {

                            var city = JsonUtility.FromJson<City>(cityjson.ToString());

                            var airports = new List<Airport>();

                            var airoportList = cityjson["Airports"];
                            if (airoportList == null) Debug.LogError("No Airports in json");
                            foreach (JSONObject airportjson in airoportList.list)
                            {
                                var airport = JsonUtility.FromJson<Airport>(airportjson.ToString());
                                airports.Add(airport);
                                string[] pos = airport.Location.Split(',');
                                airport.longitud = float.Parse(pos[0], CultureInfo.InvariantCulture.NumberFormat);
                                airport.latitud = float.Parse(pos[1], CultureInfo.InvariantCulture.NumberFormat);
                            }

                            city.Airports = airports;

                            string[] poscity = city.Location.Split(',');
                            city.longitud = float.Parse(poscity[0], CultureInfo.InvariantCulture.NumberFormat);
                            city.latitud = float.Parse(poscity[1], CultureInfo.InvariantCulture.NumberFormat);
                            cities.Add(city);
                        }
                    }
                        
                }

            }
			onFinished (true);
        }
    
    }

	public void OnGetInfoFinished(bool finished){

		if (finished)
			printCities (cities);
		else
			StartCoroutine (apiSkyCitysAndAirportsInfo (OnGetInfoFinished));

	}


	Vector2d min;
	Vector2d max;

	public void GetBoundingBoxMap(BasicMap map){
		List<UnityTile> unityTiles = transform.parent.GetComponentsInChildren<UnityTile> ().ToList ();

		min = new Vector2d (float.MaxValue , float.MaxValue);
		max = new Vector2d( float.MinValue , float.MinValue);

		foreach (UnityTile ut in unityTiles) {

			var referenceTileRect = Conversions.TileBounds(ut.UnwrappedTileId);
			Vector2d maxLatLong = Conversions.MetersToLatLon (referenceTileRect.Max);
			Vector2d minLatLong = Conversions.MetersToLatLon (referenceTileRect.Min);

			if (maxLatLong.x < min.x) min.x = maxLatLong.x;
			if (maxLatLong.x > max.x) max.x = maxLatLong.x;

			if (maxLatLong.y < min.y) min.y = maxLatLong.y;
			if (maxLatLong.y > max.y) max.y = maxLatLong.y;

			if (minLatLong.x < min.x) min.x = minLatLong.x;
			if (minLatLong.x > max.x) max.x = minLatLong.x;

			if (minLatLong.y < min.y) min.y = minLatLong.y;
			if (minLatLong.y > max.y) max.y = minLatLong.y;
		}

	}

    private void printCities(List<City> cities)
    {
		GetBoundingBoxMap (map);

        foreach (City city in cities)
        {

			if (city.latitud > max.x || city.latitud < min.x || city.longitud > max.y || city.longitud < min.y) {
				continue;
			}
            
			GameObject marcador;

			if (poolCities != null && poolCities.Count > 0) {
				marcador = poolCities.Dequeue ();
				marcador.SetActive (true);
			}
			else marcador = Instantiate(MarcadorCityPrefab);
            SetMarcadorAtWithName(marcador, new Vector2(city.latitud, city.longitud), city.Name, city);
        }
		appState = AppState.initialized;
		AudioManager.Instance.PlaySelectDepartureAudio ();
    }

    private void SetMarcadorAtWithName(GameObject marcador, Vector2 LatLong, String name, City city = null)
    {
        if (marcador == null) Debug.LogError("Marcador is null!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        var position = VectorExtensions.AsUnityPosition(LatLong, map.CenterMercator, map.WorldRelativeScale);

        marcador.transform.parent = map.Root;
        marcador.transform.localPosition = position;
        var mscript = marcador.GetComponent<Marcador>();
        if (mscript == null) Debug.LogError("Marcador SCRIPT is null!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        mscript.SetText(name);
        if (city!=null) {
            mscript.city = city;
            mscript.cityClick.RemoveAllListeners();
            mscript.cityClick.AddListener(cityMarkerClicked);
			markersGo.Add(city.IataCode ,marcador);
        }
    }
    
	public void cityMarkerClicked(GameObject go, City city){
		Debug.Log (city.Name);

		if (!String.IsNullOrEmpty(origCity.Id) && city.Id == origCity.Id) {
			go.GetComponent<Marcador> ().Deselect ();
			appState = AppState.initialized;
			AudioManager.Instance.PlaySelectDepartureAudio ();
		}
		else if (appState == AppState.initialized) {
			origCity = city;
			appState = AppState.originSelected;
			go.GetComponent<Marcador> ().Select ();
			AudioManager.Instance.PlaySelectDestinationAudio ();

		} else if (appState == AppState.originSelected) {
			endCity = city;
			appState = AppState.destinationSelected;
			go.GetComponent<Marcador> ().Select ();
			ClearCityMarkers ();
            SearchFlights();
			AudioManager.Instance.PlayLookingBestPriceAudio ();
		}

		onCityClickedEvent.Invoke (city.IataCode);
	}

	public void cityMarkerClicked(string cityId){
		GameObject go = markersGo [cityId];
		City city = go.GetComponent<Marcador> ().city;

		if (!String.IsNullOrEmpty(origCity.IataCode) && city.IataCode == origCity.IataCode) {
			go.GetComponent<Marcador> ().Deselect ();
			appState = AppState.initialized;
			AudioManager.Instance.PlaySelectDepartureAudio ();
		}
		else if (appState == AppState.initialized) {
			origCity = city;
			appState = AppState.originSelected;
			go.GetComponent<Marcador> ().Select ();
			AudioManager.Instance.PlaySelectDestinationAudio ();

		} else if (appState == AppState.originSelected) {
			endCity = city;
			appState = AppState.destinationSelected;
			go.GetComponent<Marcador> ().Select ();
			ClearCityMarkers ();
			SearchFlights();
			AudioManager.Instance.PlayLookingBestPriceAudio ();
		}
	}

    //APi REQ
    public void SearchFlights(){
        
        var data = new ApiBrowserRoutes();
        data.country = origCity.CountryId;
        data.originPlace = origCity.Id;
        data.destinationPlace = endCity.Id;
        data.outboundPartialDate = DateTime.Now.ToString("yyyy-MM-dd");
        Debug.Log("Lets call start search with data: " + data.ToString());
        routesLocator.StartSearch(data, SearchFlightsCallback);
        
	}
   
    void SearchFlightsCallback(ReturnFlightSearch data)
	{
		Debug.Log("Im back from the callback");
		if (data == null) {
			AudioManager.Instance.PlayNoRouteAudio ();
			RequestedReset ();
			return;
		}
        foreach (Airport aer in origCity.Airports)
        {
            if (aer.Id == data.IataCodeOrigin)
            {
                origenAirport = aer;
                break;
            }
        }
        if (origenAirport == null)
            origenAirport = origCity.Airports.First();
        foreach (Airport aer in endCity.Airports)
        {
            if (aer.Id == data.IataCodeEnd)
            {
                endAirport = aer;
                break;
            }
        }
        if (endAirport == null) {
            endAirport = endCity.Airports.First();
        }

        GameObject marcadorOrigen = Instantiate(MarcadorAirportPrefab);
        SetMarcadorAtWithName(marcadorOrigen, new Vector2(origenAirport.latitud, origenAirport.longitud), origenAirport.Name);
        GameObject marcadorDesti = Instantiate(MarcadorAirportPrefab);
        SetMarcadorAtWithName(marcadorDesti, new Vector2(endAirport.latitud, endAirport.longitud), endAirport.Name);

        beizerLaser = Instantiate(BaizerLine);
        beizerLaser.transform.parent = map.Root;

        var bezierFunc = beizerLaser.GetComponentInChildren<BezierPointSetter>();


        float distanceBetweenPoints = bezierFunc.SetPoints(marcadorOrigen.transform.position, marcadorDesti.transform.position);

        GameObject paperPlane = Instantiate(PlanePrefab);
        paperPlane.transform.position = marcadorOrigen.transform.position;

        iTween.MoveFrom(paperPlane, iTween.Hash("path",beizerLaser.GetComponent<CBezier>().Points ,"position", marcadorDesti.transform.position, "islocal", true, "time", distanceBetweenPoints * 150, "orienttopath", true));
        //iTween.MoveTo(paperPlane, marcadorDesti.transform.position, distanceBetweenPoints*150);

		AudioManager.Instance.PlayFlightOkeyAudio ();
        print("Print Num: " + data);
    }

    public void RequestedReset()
    {
        Debug.Log("some");
        ClearCityMarkers();
        ClearAirportMarkers();
        origCity = new City();
        endCity = new City();
        if(beizerLaser != null) {
            if( beizerLaser.GetComponent<BezierPointSetter>() != null) beizerLaser.GetComponent<BezierPointSetter>().ClearPoints();
        }
        printCities(cities);
        if (onResetEvent != null) onResetEvent.Invoke();
    }

    public void RemoteReset(){
        ClearCityMarkers();
        ClearAirportMarkers();
        origCity = new City();
        endCity = new City();
        if (beizerLaser != null)
        {
            if (beizerLaser.GetComponent<BezierPointSetter>() != null) beizerLaser.GetComponent<BezierPointSetter>().ClearPoints();
        }
        printCities(cities);
    }

	public void ClearCityMarkers(){
		List<string> keys = markersGo.Keys.ToList ();
		foreach (string k in keys) {
			GameObject g = markersGo [k];
			g.SetActive (false);
            g.GetComponent<Marcador>().Deselect(); 
			poolCities.Enqueue (g);
		}
		markersGo = new Dictionary<string, GameObject>();
	}

	public void ClearAirportMarkers(){
		List<string> keys = airportsGo.Keys.ToList ();
		foreach (string k in keys) {
			GameObject g = airportsGo [k];
			Destroy (g);
		}
		airportsGo = new Dictionary<string, GameObject> ();

	}

}


public enum AppState{
		initializing,
		initialized,
		originSelected,
		destinationSelected,
		showingResults
}