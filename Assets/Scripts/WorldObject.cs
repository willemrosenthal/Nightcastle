using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (Velocity))]
public class WorldObject : MonoBehaviour {

    public bool checkCollisionWithPlayer = false;
    public bool pushable = false;

    // collisions with player
    bool collidingWithPlayer;

    // internal references
    [HideInInspector] public Controller2D controller;
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public SpriteAnim spriteAnim;
    [HideInInspector] public Health health;
    [HideInInspector] public Gravity gravity;
    [HideInInspector] public Velocity velocity;

    // external references
    [HideInInspector] public CameraBounds cameraBounds;
    [HideInInspector] public GameManager gm;
    [HideInInspector] public Player player;

    void Start() {
        player = Player.Instance;
        controller = GetComponent<Controller2D>();
        sr = GetComponent<SpriteRenderer>();
        spriteAnim = GetComponent<SpriteAnim>();
        cameraBounds = Camera.main.GetComponent<CameraBounds>();

        // get potential refs
        health = GetComponent<Health>();
        gravity = GetComponent<Gravity>();
        velocity = GetComponent<Velocity>();
    }

    void Update() {
        // dont excute if far away
        if (Vector2.Distance(transform.position, Camera.main.transform.position) > 100) return;

        // Gravity
        if (gravity) gravity.ApplyGravity();

        // where enemy code is written
        ObjectUpdate();

        // code for collisions with player
        if (checkCollisionWithPlayer) PlayerCollisions();

        // move enemy
        controller.Move(velocity.v * GTime.deltaTime);
    }

    // put update content here
    public virtual void ObjectUpdate() {}

    public void PlayerCollisions () {
        //overlaping with player
        if (Collision.Check(player.controller, this)) {
            if (! collidingWithPlayer) {
                EnterPlayerCollision();
                collidingWithPlayer = true;
            }
        }
        // not overlapping with player
        else {
            if (collidingWithPlayer) {
                ExitPlayerCollision();
            }
        }
    }

    public virtual void EnterPlayerCollision() {}

    public virtual void ExitPlayerCollision() {}
}
