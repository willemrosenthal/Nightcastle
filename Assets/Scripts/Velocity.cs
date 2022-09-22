using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Velocity : MonoBehaviour {

    [HideInInspector] public float x;
    [HideInInspector] public float y;
    [HideInInspector] public Vector2 last;

    //public Vector2 _v;
    public Vector2 v {
        get {
            return new Vector2(x, y);
        }
        set {
            x = value.x;
            y = value.y;
        }
    }



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


    public Vector2 Get() {
        return new Vector2(x,y);
    }
    
    public void Set(Vector2 _velcoity) {
        x = _velcoity.x;
        y = _velcoity.y;
    }

    private void LateUpdate() {
        last = v;    
    }

}
