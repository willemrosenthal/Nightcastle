using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour {

	public static GameResources Instance { get; private set; }

    // how this should work:
    // the first time we try to get a dialouge for a given language, it checks a dictionary to see if that font ref is saved.
    // if it is NOT saved, load from resources and create a key-value-pair.
    // if it IS saved, just use the key-value pair to retrieve it.
    public GameObject dialougeFont; 




    void Awake() {
        Singleton();
    }
    void Singleton () {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy (gameObject);
		}
	}

    
}
