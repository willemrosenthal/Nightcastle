using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {
    
    [HideInInspector]public float gravity;
    [HideInInspector]public Controller2D controller;
    public bool snapToGroundOnStart = false;
    public float initialHangtime = 0;

    void Start() {
        gravity = World.gravity;
    }

    public void Initilize( Controller2D _controller ) {
        controller = _controller;
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
        velocity.y += gravity * GTime.deltaTime;
    }
}
