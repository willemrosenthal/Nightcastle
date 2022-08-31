using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : MonoBehaviour {

    public bool debug;
    Dictionary<Transform, WorldObject> pushObjectDictionary = new Dictionary<Transform, WorldObject>();

    Controller2D controller;
    GameManager gm;
    Zone currentZone;

    void Start() {
        gm = GameManager.Instance;
        controller = GetComponent<Controller2D>();
    }

    // happens after normal update, this way we can be confient that side collisions are left intact
    void LateUpdate() {
        if (controller.collisions.left || controller.collisions.right) {
            if (debug) Debug.Log("pushing: " + controller.collisions.sideCollisionObject.name);
            if (!pushObjectDictionary.ContainsKey(controller.collisions.sideCollisionObject)) {
                pushObjectDictionary.Add(controller.collisions.sideCollisionObject, controller.collisions.sideCollisionObject.GetComponent<WorldObject>());
            }
            WorldObject pushableObj = pushObjectDictionary[controller.collisions.sideCollisionObject];
            if (pushableObj && pushableObj.pushable && controller.collisions.below) {
                Vector2 pushVelocity = controller.collisions.left ? new Vector2(-1,-0.1f) :  new Vector2(1,-.1f);
                pushableObj.controller.Move(pushVelocity * GTime.deltaTime);
                // play push sound refercend in the WorldObject
            }
        }

        // reset deictionary if zone changes
        if (currentZone != gm.GetCurrentZone()) {
            currentZone = gm.GetCurrentZone();
            pushObjectDictionary = new Dictionary<Transform, WorldObject>();
        }
    }
}
