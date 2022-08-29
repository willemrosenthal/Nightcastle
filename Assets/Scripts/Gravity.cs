using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {
    
    public float gravity;
    [HideInInspector]public Controller2D controller;

    void Start() {
        gravity = World.gravity;
    }

    public void ApplyGravity(ref Vector2 velocity) {
        // cancel out accumulation of velocity if you hit something vertically
        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }
        velocity.y += gravity * GTime.deltaTime;
    }
}
