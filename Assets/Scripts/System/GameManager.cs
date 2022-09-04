using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class GameManager : MonoBehaviour {
    // singleton
	public static GameManager Instance { get; private set; }

    // current zone
    Zone currentZone;

    // refs
    [HideInInspector] public GameSettings settings;
    [HideInInspector] public InputManager im;
    [HideInInspector] public PlayerInputs playerInputs;
    [HideInInspector] public GameTime time;
    [HideInInspector] public CameraBounds cameraBounds;
    [HideInInspector] public CameraFollow cameraFollow;
    [HideInInspector] public AudioManager am;

    void Awake() {
        // set up as singleton, 
        Singleton();
        // if a duplicate, end code here
		if (Instance != this) return;

        // get refs to stuff attached to the GameManager object
        settings = GetComponent<GameSettings>();
        im = GetComponent<InputManager>();
        playerInputs = im.playerInputs;
        time = GetComponent<GameTime>();
        am = GetComponent<AudioManager>();
    }

    void Singleton () {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy (gameObject);
		}
	}

    void Start() {
        ConfigureQuality();
    }

    // zone
    public void SetCurrentZone(Zone zone) {
        currentZone = zone;
    }
    public Zone GetCurrentZone() {
        return currentZone;
    }

    void ConfigureQuality() {
        // do quality stuff
        if (Screen.currentResolution.refreshRate < 75) QualitySettings.vSyncCount = 1;
		else if (Screen.currentResolution.refreshRate < 130) QualitySettings.vSyncCount = 2;
		else if (Screen.currentResolution.refreshRate > 130) QualitySettings.vSyncCount = 0;
		//Debug.Log("VSYNC set to " + QualitySettings.vSyncCount);

        // fps
		Application.targetFrameRate = (int)time.fps;
    }
}
