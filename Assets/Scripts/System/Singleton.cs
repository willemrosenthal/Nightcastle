using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    public MonoBehaviour singletonClass;
	public static MonoBehaviour Instance  { get; private set; }

    void Awake() {
        // set up as singleton, 
        MakeSingleton();
    }

    void MakeSingleton () {
		if (Instance == null) {
			Instance = singletonClass;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy (gameObject);
		}
	}
}
