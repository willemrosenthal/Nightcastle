using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {
    
    [HideInInspector]public float gravity;
    [HideInInspector]public Controller2D controller;
    public InWater inWater;
    public bool snapToGroundOnStart = false;
    public float initialHangtime = 0;

    void Start() {
        gravity = World.gravity;
        controller = GetComponent<Controller2D>();
        inWater = GetComponent<InWater>();
        Initilize();
    }

    public void Initilize() {
        if (snapToGroundOnStart) {
            controller.Move(Vector2.down * 5);
        }
    }

    public void ApplyGravity(ref Vector2 velocity) {
        // objects hang in air for a moment before gravity takes effect. used in death animations mostly
        if (initialHangtime > 0) {
            initialHangtime -= GTime.deltaTime;
            return;
        }
        // cancel out accumulation of velocity if you hit something vertically
        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

        // in water, dampen gravity
        float inWaterDampen = 1;
        if (inWater && inWater.inWater) inWaterDampen = inWater.dampenGravity;

        velocity.y += gravity * GTime.deltaTime * inWaterDampen;
    }
}
