using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;




[Serializable]

public class CityClick : UnityEvent <GameObject, City> {}


[Serializable]

public class CityIDClick : UnityEvent <string> {}


