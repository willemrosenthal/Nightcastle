using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InWater : MonoBehaviour {
    public bool inWater;
    public float ySwimPointAdjust = 0.45f;
    public float dampenMovementX = 0.65f;
    public float dampenMovementY = 0.5f;

    public float _dampenGravity = 0.3f;
    public float dampenGravity {
        get {
            if (boyant && controller && floating) return _dampenGravity * 0.5f;
            if (controller && bounds.min.y < water.position.y) return 1;
            return _dampenGravity;
        }
        set {
            _dampenGravity = value;
        }
    }

    Bounds bounds;

    // BOYANCY
    public bool boyant;
    [HideInInspector] public bool jumpOk = false;
    public bool floating;
    float boyantVelocity;
    float boyantGravity = 10;

    public Transform water; 
    Controller2D controller;
    Velocity velocity;

    void Start() {
        controller = GetComponent<Controller2D>();
        velocity = GetComponent<Velocity>();
        bounds = controller.raycastOrigins.bounds;
    }


    public void Update() {
        jumpOk = false;
        floating = false;
        // NEED TO CANCEL OUT Y VELOCITY

        if (inWater && boyant && controller) {
            // update bounds center
             bounds = controller.colliderBox.bounds;

             float depth = water.position.y - (bounds.center.y + ySwimPointAdjust);

            // if propperly submerged
            if (bounds.center.y + ySwimPointAdjust < water.position.y) {
                floating = true;

                boyantVelocity += boyantGravity * GTime.deltaTime * depth * 2;
                controller.Move(Vector2.up * boyantVelocity * GTime.deltaTime);
                
                if (velocity.y < 0.5f) {
                    velocity.y += boyantVelocity;
                }
                jumpOk = true;
            }
            // othewrise, no boyant velocity and think about weather jumping is ok
            else {
                boyantVelocity = 0;
                // // still must be partly submerged to get a jump
                if (Mathf.Lerp(bounds.center.y + ySwimPointAdjust, bounds.min.y, 0.33f) <= water.position.y) {
                    jumpOk = true;
                }
            }
            // still considered floating if half in water
            if (bounds.center.y <= water.position.y) floating = true;
        }
        else {
            boyantVelocity = 0;
        }
    }


    // OLD!
    // public void Update() {
    //     jumpOk = false;
    //     floating = false;
    //     // NEED TO CANCEL OUT Y VELOCITY

    //     if (inWater && boyant && controller) {
    //         // update bounds center
    //         bounds = controller.colliderBox.bounds;

    //         // if propperly submerged
    //         if (bounds.center.y + ySwimPointAdjust <= water.position.y) {
    //             floating = true;
    //             boyantVelocity += boyanyGravity * GTime.deltaTime;
    //             controller.Move(Vector2.up * boyantVelocity * GTime.deltaTime);
    //             if (velocity.y < 0.5f) velocity.y += boyantVelocity;
    //             jumpOk = true;
    //         }
    //         // othewrise, no boyant velocity and think about weather jumping is ok
    //         else {
    //             boyantVelocity = 0;
    //             // still must be partly submerged to get a jump
    //             if (Mathf.Lerp(bounds.center.y + ySwimPointAdjust, bounds.min.y, 0.33f) <= water.position.y) {
    //                 jumpOk = true;
    //             }
    //         }
    //         // still considered floating if half in water
    //         if (bounds.center.y <= water.position.y) floating = true;
            
    //     }
    //     else {
    //         boyantVelocity = 0;
    //     }
    // }

}
