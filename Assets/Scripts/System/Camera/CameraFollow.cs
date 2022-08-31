using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // will be CameraMan // cameraman should follow the players's center point by default.

    // where camera should be
    public Vector2 lookPos;

    // bounds to constrain camera
    Zone currentZone;
    public Bounds zoneBounds;
    
    CameraBounds cameraBounds;
    GameManager gm;

    void Awake() {
        gm = GameManager.Instance;
        gm.cameraFollow = this;
    }

    void Start() {
        cameraBounds = GetComponent<CameraBounds>();
    }

    public void CameraUpdate() {
        // makes sure bounds are up to date
        cameraBounds.UpdateBounds();

        // get where we want to be looking
        lookPos = (Vector2) target.transform.position;

        // constrain to bounds
        ConstrainCameraToBounds();

        // sets camera to look position
        transform.position = AddZDepth (lookPos);
    }

    void ConstrainCameraToBounds () {
        // get 2D camera bounds
        Bounds camBounds = cameraBounds.bounds2D;
        // orient bounds's center where we want it
        camBounds.center = lookPos;
        // offset to fit camera in bounds
        Vector2 offset = Vector2.zero;

        // if camSize is bigger than bounds, center camera in bounds
        if (zoneBounds.size.x <= camBounds.size.x) {
            offset.x = zoneBounds.center.x - camBounds.center.x;
        }
        else {
            // if cam left of bounds
            if (camBounds.min.x < zoneBounds.min.x) offset.x = zoneBounds.min.x - camBounds.min.x;
            // if cam right of bounds
            if (camBounds.max.x > zoneBounds.max.x) offset.x = zoneBounds.max.x - camBounds.max.x;
        }
        // if camSize is bigger than bounds, center camera in bounds
        if (zoneBounds.size.y <= camBounds.size.y) {
            offset.y = zoneBounds.center.y - camBounds.center.y;
        }
        else {
            // if cam is below bounds
            if (camBounds.min.y < zoneBounds.min.y) offset.y = zoneBounds.min.y - camBounds.min.y;
            // if cam is above bounds
            if (camBounds.max.y > zoneBounds.max.y) offset.y = zoneBounds.max.y - camBounds.max.y;
        }
        
        // add offset to look pos
        lookPos += offset;
    }

    public void SetZoneBounds(Zone zone) {
        zoneBounds = zone.GetBounds();
    }

    Vector3 AddZDepth (Vector2 pos) {
        return (Vector3)pos + Vector3.forward * -10;
    }
}
