using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

[RequireComponent (typeof (SpriteRenderer))]
[RequireComponent (typeof (SpriteAnim))]
[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (Health))]
[RequireComponent (typeof (Velocity))]
public class Enemy : MonoBehaviour {
    // all objects should track what camera zone they are in. If the palyer leaves that zone, they should despawn (or reset)

    // hurt player when player touches
    // move speed
    // gravity
    // jump height
    // jump x velocity
    // ref to player
    // activate when on camera

    // make shake component?
    public Vector2 hitstopShake = new Vector2(1f,0);
    int shakeDir = 1;

    // internal values
    [HideInInspector] public bool activated;
    [HideInInspector] public Bounds spriteBounds;

    // collisions with player
    bool collidingWithPlayer;

    // references
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
        gm = GameManager.Instance;
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
        UpdateSpriteBounds();

        // stay inactive and don't progress in the code if off-camera
        if (!activated) {
            if (OnCamera()) activated = true;
            else return;
        }

        // Gravity
        if (gravity) gravity.ApplyGravity();

        // Hitstop
        if (health.Histop()) {
            Hitstop();
            return;
        }

        // where enemy code is written
        EnemyUpdate();

        // code for collisions with player
        PlayerCollisions();

        // move enemy
        controller.Move(velocity.v * GTime.deltaTime);
    }

    public virtual void Hitstop() {
        if (hitstopShake != Vector2.zero) {
            Vector2 shakeAmount = hitstopShake * (1f/32f) * (float)shakeDir;
            shakeDir *= -1;
            controller.Move(shakeAmount);
        }
    }

    public virtual void EnemyUpdate() {
        FacePlayer();
    }

    public void FacePlayer() {
        FaceDir( (int) Mathf.Sign(player.transform.position.x - transform.position.x) );
    }

    public void FaceDir( int dir ) {
        dir = Mathf.Clamp(dir, -1, 1);
        if (dir == 0) return;
        transform.localScale = new Vector3 ( Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y, transform.localScale.z);
    }

    public float GetFacing() {
        return Mathf.Sign(transform.localScale.x);
    }

    void UpdateSpriteBounds() {
        spriteBounds = sr.bounds;
        spriteBounds.center = (Vector2)spriteBounds.center;
    }

    bool OnCamera() {
        return cameraBounds.bounds2D.Intersects(spriteBounds);
    }


    // extrude into obj collisions class?
    void PlayerCollisions () {
        //overlaping with player
        if (Collision.Check(player, this)) {
            if (! collidingWithPlayer) {
                EnterPlayerCollision();
                collidingWithPlayer = true;
            }

            player.health.LooseHealth(1);
        }
        // not overlapping with player
        else {
            if (collidingWithPlayer) {
                ExitPlayerCollision();
            }
            collidingWithPlayer = false;
        }
    }

    public virtual void EnterPlayerCollision() {
        Debug.Log ("Just collided with player");
    }

    public virtual void ExitPlayerCollision() {
        Debug.Log ("Just collided with player");
    }


    // GIZMOS
    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spriteBounds.center, spriteBounds.size);

        if (Application.isPlaying && enabled) {
            Gizmos.color = Color.red;
            if (controller) {
                Gizmos.DrawWireCube(controller.colliderBox.bounds.center, controller.colliderBox.bounds.size);
            }
        }
    }
}
