using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public static class GTime {
[DefaultExecutionOrder(-1000)]
public class GameTime : MonoBehaviour {

	public static GameTime Instance { get; private set; }

    // time settings
    public bool fixedTime;
    public int fps = 60;

    // time scale
    float timeScale = 1;

    // time 
    float deltaTime;
    float unscaledDeltaTime;
    float time;
    float unscaledTime;

    // internal
    bool fixedModeSetupComplete;
    float frameTime;
    float _fps;
    float fixedTimeGameTimeDiff = 0;
    float fixedTimeGameUnscaledTimeDiff = 0;

    void Awake() {
        Singleton();
		if (Instance != this) return;

        SetFPS();
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

    void SetFPS() {
        if (fps <= 0) {
            Debug.LogError("FPS can not be set to 0");
            Debug.Break();
        }
        //print("FPS recalculated");
        _fps = fps;
        frameTime = 1f/(float)fps;
        if (fixedTime) SetFixed();
        Application.targetFrameRate = (int)fps;
    }

    void SetFixed() {
        //print("fixed time turned on");
        fixedModeSetupComplete = true;
        deltaTime = frameTime;
        unscaledDeltaTime = frameTime;
    }

    void UnsetFixed() {
        //print("fixed time turned off");
        fixedModeSetupComplete = false;
        fixedTimeGameTimeDiff = time - UnityEngine.Time.time;
        fixedTimeGameUnscaledTimeDiff = unscaledTime - UnityEngine.Time.unscaledTime;
    }

    void Update() {
        // if fps gets changed
        if (_fps != fps) SetFPS();

        // sets fixed time if toggled on durring gameplay
        if (!fixedModeSetupComplete && fixedTime) SetFixed();
        if (fixedModeSetupComplete && !fixedTime) UnsetFixed();

        // if fixed time is off, use retular time
        if (!fixedTime) {
            unscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime;
            deltaTime = UnityEngine.Time.deltaTime;
            time = UnityEngine.Time.time + fixedTimeGameTimeDiff;
            unscaledTime = UnityEngine.Time.unscaledTime + fixedTimeGameUnscaledTimeDiff;
        }
        // use fixed time
        else {
            deltaTime = frameTime * timeScale;
            time += deltaTime;
            unscaledTime += frameTime;
        }

        // updates time values
        GTime.UpdateTime(deltaTime, unscaledDeltaTime, time, unscaledTime);
    }

    public void UpdateTimeScale(float _timeScale) {
        // set the game's timescale. This is called from the setting the timescale of GTime 
        timeScale = _timeScale;
        Time.timeScale = _timeScale;
    }
}
