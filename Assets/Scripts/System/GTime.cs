using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GTime {

    public static float deltaTime;
    public static float unscaledDeltaTime;
    public static float time;
    public static float unscaledTime;

    public static float deltaTimePrevious;
    public static float unscaledDeltaTimePrevious;

    public static float _timeScale = 1;
    public static float timeScale {
        get {
            return _timeScale;
        }
        set {
            _timeScale = value;
            GameTime.Instance.UpdateTimeScale(_timeScale);
        }
    }

    public static void UpdateTime(float _deltaTime, float _unscaledDeltaTime, float _time, float _unscaledTime) {
        // set previous deltatimes
        deltaTimePrevious = deltaTime;
        unscaledDeltaTimePrevious = unscaledDeltaTime;

        // update deltatimes to new times
        deltaTime = _deltaTime;
        unscaledDeltaTime = _unscaledDeltaTime;
        time = _time;
        unscaledTime = _unscaledTime;

        // update world time
        WorldTime.UpdateTime(deltaTime);
    }

}
