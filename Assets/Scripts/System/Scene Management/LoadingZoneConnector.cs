using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingZoneConnector : MonoBehaviour {

	// Dictionary<string, Transform> loadingZoneList = new Dictionary<string, Transform>();

	public static LoadingZoneConnector Instance { get; private set; }

    void Awake() {
        // set up as singleton, 
        Singleton();
        // if a duplicate, end code here
		if (Instance != this) return;
    }

    void Singleton () {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy (gameObject);
		}
	}

    [HideInInspector] public string triggeredID;
    [HideInInspector] public string triggeredScene;
    [HideInInspector] public Transform triggredZone;
}
