using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.XR.iOS;

namespace sw33.UnityEvents{


	[Serializable]

	public class ArPlaneHit : UnityEvent <List<ARHitTestResult>> {}

}
