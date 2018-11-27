using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReturnFlightSearch
{
    public string IataCodeOrigin;
    public string IataCodeEnd;
    public int minPrize;
}

public class RoutesLocator : MonoBehaviour {

    public static string apiKey = "ha952718948176934114947694895142";
    public List<Quotes> quotes;
    public List<Places> places;
    public ApiBrowserRoutes apiBrowserRoutes;
    public Quotes q;
    public Places placeOrigin;
    public Places placeDestination;

    protected ReturnFlightSearch callbackFct;

    public delegate void MyDelegate(ReturnFlightSearch num);
    // Use this for initialization
    public void StartSearch(ApiBrowserRoutes data, MyDelegate myDelegate) {
        quotes = new List<Quotes>();
        places = new List<Places>();
        q = new Quotes();
        placeOrigin = new Places();
        placeDestination = new Places();
        apiBrowserRoutes = data;

        Debug.Log("Hello");
        StartCoroutine(callSkyApiQuotes(myDelegate));
    }

    private IEnumerator callSkyApiQuotes(MyDelegate callback)
    {
        var path = "http://partners.api.skyscanner.net/apiservices/browseroutes/v1.0/" +
           apiBrowserRoutes.country + "/" +
           apiBrowserRoutes.currency + "/" +
           apiBrowserRoutes.locale + "/" +
           apiBrowserRoutes.originPlace + "/" +
           apiBrowserRoutes.destinationPlace + "/" +
           apiBrowserRoutes.outboundPartialDate + 
            "?apiKey=" + apiKey;
        Debug.Log(path);
        UnityWebRequest www = UnityWebRequest.Get(path);

        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
			callback (null);
        }
        else
        {
            ReturnFlightSearch response = new ReturnFlightSearch();
            Debug.Log("Here");
            var json = new JSONObject(www.downloadHandler.text);
            //Debug.Log(json);
            var quotelist = json["Quotes"];
            if(quotelist == null || quotelist.list == null || quotelist.list.Count <= 0)
            {
                Debug.Log("No hi ha viatges");
				callback(null);
                yield return 0;
            }
            int minPriceFlight = int.MaxValue;
            int quoteMinPrice;
            foreach (JSONObject quote in quotelist.list)
            {
                Int32.TryParse(quote["MinPrice"].ToString(), out quoteMinPrice);
                if (quoteMinPrice < minPriceFlight)
                {
                    Int32.TryParse(quote["QuoteId"].ToString(), out q.QuoteId);
                    Int32.TryParse(quote["MinPrice"].ToString(), out q.MinPrice);
                    Boolean.TryParse(quote["Direct"].ToString(), out q.Direct);
                    var auxJson = quote["OutboundLeg"];
                    Int32.TryParse(auxJson["OriginId"].ToString(), out q.originId);
                    Int32.TryParse(auxJson["DestinationId"].ToString(), out q.destinationId);
                    q.departureDate = (auxJson["DepartureDate"].ToString());
                    Int32.TryParse(quote["MinPrice"].ToString(), out minPriceFlight);
                }
            }
            Debug.Log(" price: " + q.MinPrice);
            var placeslist = json["Places"];
            response.minPrize = q.MinPrice;
            int cityOrigin;
            int cityDestination;
            foreach (JSONObject place in placeslist.list)
            {
                Int32.TryParse(place["PlaceId"].ToString(), out cityOrigin);
                Int32.TryParse(place["PlaceId"].ToString(), out cityDestination);
                if (cityOrigin == q.originId)
                {
                    placeOrigin.PlaceId = cityOrigin;
                    placeOrigin.IataCode = place["IataCode"].str;
                    placeOrigin.Name = place["Name"].str;
                    placeOrigin.Type = place["Type"].str;
                    placeOrigin.SkyscannerCode = place["SkyscannerCode"].str;
                    placeOrigin.CityName = place["CityName"].str;
                    Int32.TryParse(place["CityId"].ToString(), out placeOrigin.CityId);
                    placeOrigin.CountryName = place["CountryName"].ToString();
                    response.IataCodeOrigin = placeOrigin.IataCode;
                }
                if (cityDestination == q.destinationId)
                {
                    placeDestination.PlaceId = cityDestination ;
                    placeDestination.IataCode = place["IataCode"].str;
                    placeDestination.Name = place["Name"].str;
                    placeDestination.Type = place["Type"].str;
                    placeDestination.SkyscannerCode = place["SkyscannerCode"].str;
                    placeDestination.CityName = place["CityName"].str;
                    Int32.TryParse(place["CityId"].ToString(), out placeDestination.CityId);
                    placeDestination.CountryName = place["CountryName"].str;
                    response.IataCodeEnd = placeDestination.IataCode;
                }
            }
            Debug.Log("Place Origin id: " + placeOrigin.PlaceId);
            Debug.Log("Place Destination id: " + placeDestination.PlaceId);
            Debug.Log("Lets Call Callback");
            callback(response);
        }

    }

    // Update is called once per frame
    void Update () {
		
	}

    private void modificarValorsEntrada()
    {  
        apiBrowserRoutes.country = "ES";
        apiBrowserRoutes.originPlace = "BCN";
        apiBrowserRoutes.destinationPlace = "LOND";
        apiBrowserRoutes.outboundPartialDate = "2017-10-15";
        apiBrowserRoutes.inboundPartialDate = "";
    }
}
