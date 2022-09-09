using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InWater : MonoBehaviour {
    public bool inWater;
    public float dampenMovementX = 0.65f;
    public float dampenMovementY = 0.5f;

    public float _dampenGravity = 0.3f;
    public float dampenGravity {
        get {
            if (boyant && controller && bounds.center.y < water.position.y) return 0;
            if (controller && bounds.min.y < water.position.y) return 1;
            return _dampenGravity;
        }
        set {
            _dampenGravity = value;
        }
    }

    Bounds bounds;

    float _normalGravityDampen;

    public bool boyant;
    float boyantVelocity;
    float boyanyGravity = 6;
    public Transform water; 

    public Controller2D controller;

    Player player;
    Enemy enemy;
    WorldObject worldObject;

    void Start() {
        controller = GetComponent<Controller2D>();
        bounds = controller.raycastOrigins.bounds;
        //
        // player = GetComponent<Player>();
        // enemy = GetComponent<Enemy>();
        // worldObject = GetComponent<WorldObject>();
    }

    public void Update() {
        // NEED TO CANCEL OUT Y VELOCITY

        if (inWater && boyant && controller) {
            // update bounds center
            bounds = controller.raycastOrigins.bounds;

            if (bounds.center.y <= water.position.y) {
                boyantVelocity += boyanyGravity * GTime.deltaTime;
                controller.Move(Vector2.up * boyantVelocity * GTime.deltaTime);
                controller.collisions.below = true;
            } else {
                boyantVelocity = 0;

                // still must be partly submerged to get a jump
                if (Mathf.Lerp(bounds.center.y, bounds.min.y, 0.33f) <= water.position.y) {
                    controller.collisions.below = true;
                }
            }
        }
    }

}
