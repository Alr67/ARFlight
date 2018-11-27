using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class City
{
    public bool SingleAirportCity;
    public List<Airport> Airports;
    public String CountryId;
    public String RegionId;
    public String Location;
    public String IataCode;
    public String Name;
    public String Id;
    public float latitud;
    public float longitud;
}
