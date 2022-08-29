using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour {

    PixelPerfectRenderer ppr;
    public Bounds bounds;
    public Bounds bounds2D;


    void Start() {
       ppr = GetComponent<PixelPerfectRenderer>(); 
       bounds = new Bounds(transform.position, ppr.GetCameraSize());
       bounds2D = bounds;
    }


    void Update() {
        //bounds = new Bounds(transform.position, ppr.GetCameraSize());
        UpdateBounds();
    }

    public void UpdateBounds() {
        bounds.center = transform.position;
        bounds2D.center = (Vector2) bounds.center;
    }

    void OnDrawGizmos() {
        if (Application.isPlaying) {
            // Gizmos.color = new Color(1,0,0,0.33f);
            // Gizmos.DrawCube(transform.position, bounds.size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, bounds.size);
        }
    }

}
