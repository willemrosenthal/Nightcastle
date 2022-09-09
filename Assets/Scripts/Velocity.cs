using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Velocity : MonoBehaviour {

    [HideInInspector] public float x;
    [HideInInspector] public float y;
    [HideInInspector] public Vector2 last;

    public Vector2 v {
        get {
            return new Vector2(x, y);
        }
        set {
            v = value;
            x = v.x;
            y = v.y;
        }
    }


    public Vector2 Get() {
        return new Vector2(x,y);
    }
    public void Set(Vector2 _velcoity) {
        x = _velcoity.x;
        y = _velcoity.y;
        v = _velcoity;
    }

    private void LateUpdate() {
        last = v;    
    }

}
