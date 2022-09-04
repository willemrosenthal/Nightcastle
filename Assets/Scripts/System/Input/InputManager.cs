using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
#if UNITY_SWITCH && !UNITY_EDITOR
using nn.hid;
#endif

[DefaultExecutionOrder(-75)]
public class InputManager : MonoBehaviour
{
    // basic input stuff
    public static InputManager Instance { get; private set; }

	public PlayerInputs playerInputs;
    GameManager gm;

    void Awake() {
        Singleton();
		if (Instance != this) return; // stop code if it is a duplicate

        // get player input bindings
		playerInputs = PlayerInputs.CreateWithDefaultBindings(); 
    }

    void Singleton () {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy (gameObject);
        }
    }

    void Start() {
        gm = GameManager.Instance;
    }

}
