using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (Health))]
[RequireComponent (typeof (PlayerAnimation))]
public class Player : MonoBehaviour {

    // singleton
	public static Player Instance { get; private set; }
    
    // jumping
    public float jumpHeight = 3.5f;
    public float timeToJumpApex = 0.4f;
    float accelerationTimeAirborne = 0.2f;
    float accelerationTimeGrounded = 0.01f;
    public float moveSpeed = 2.5f;

    // coyote time
    bool coyoteReady;
    float coyoteTime = 0.1f;
    float coyoteTimer;

    // variable jump velocity
    public float minJumpHeight = 1;
    bool jumpHeald;
    float minJumpGravityMultiplier;

    // wall jump
    public bool allowWallJump = true;
    public Vector2 wallJumpClimb = new Vector2(4f, 20f);
    public Vector2 wallJumpOff = new Vector2(4f, 5f);
    public Vector2 wallLeap = new Vector2(13f, 16f);

    public float wallSlideSpeedMax = 1.5f;
    public float wallStickTime = 0.2f;
    float timeToWallUnstick;

    // gravity and jump internal variables
    float gravity;
    float jumpVelocity;

    // fall hard landing
    float fallTime;
    float longFallTime = 0.55f;

    // velocity
    Vector3 velocity;
    float velocityXSmoothing;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;

    // sliding down slope
    bool slidingDownMaxSlope;


    [HideInInspector] public Controller2D controller;
    [HideInInspector] public Health health;
    PlayerAnimation animate;

    GameManager gm;
    PlayerInputs playerInputs;

    void Awake() {
        // singleton and if a duplicate, end code here
        Singleton();
		if (Instance != this) return;
        
        // get refs
        gm = GameManager.Instance;
        playerInputs = gm.playerInputs;
    }

    void Singleton () {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy (gameObject);
		}
	}

    void Start() {
        health = GetComponent<Health> ();
        controller = GetComponent<Controller2D> ();
        animate = GetComponent<PlayerAnimation> ();
        //controller.SetDistBetweenRays(0.25f); // sets rays super close for player

        // calculate gravity based on desired jump height and time
        gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
        World.SetGravity(gravity);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        // calculate min jump hight diff for variable jump heights
        minJumpGravityMultiplier = jumpHeight / minJumpHeight;
    }

    void Update() {
        // get directional input
        directionalInput = playerInputs.Dpad;

        CalculateVelocity();
        HandleWallSliding();
        HandleJumping();
        HandleAttack();
        DebugInstantReset();
        if (FallHard ()) {
            return;
        }

        controller.Move(velocity * GTime.deltaTime);

        if (controller.collisions.above || controller.collisions.below) {
            if (controller.collisions.slidingDownMaxSlope) {
                if (!slidingDownMaxSlope) velocity.y = 0; //*= 0.15f; // if you already have a high velocity downward - reduce it on collision with slope before sliding
                velocity.y += controller.collisions.slopeNormal.y * -gravity * GTime.deltaTime;
            }
            else {
                velocity.y = 0;
            }
        }
        slidingDownMaxSlope = controller.collisions.slidingDownMaxSlope;

        animate.HandleAnimations(velocity, directionalInput);
    }

    public bool attacking = false;
    void HandleAttack () {
        if (!attacking) {
            if (playerInputs.Command.WasPressed && controller.collisions.below) {
                animate.PlayAnimation(animate.attackStanding);
                attacking = true;
                velocity.x = 0;
            }
        }
        if (attacking) {
            if (!animate.IsPlaying(animate.attackStanding)) attacking = false;
            velocity.x = 0;
            if (velocity.y > 0) velocity.y = 0;
        }
         
    }

    public void OnJumpInputDown() {
        //Debug.Log("JUMP ATTEMPT: " + controller.collisions.below);
        if (wallSliding) {
            // fall off wall
            if (directionalInput.x == 0) {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            // climb jump
            else if (wallDirX == Mathf.Sign(directionalInput.x)) {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            // fall off wall
            else {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }
        if (controller.collisions.below || coyoteTimer > 0) {
            if (controller.collisions.slidingDownMaxSlope) {
                velocity.y = jumpVelocity * controller.collisions.slopeNormal.y;
                velocity.x = jumpVelocity * controller.collisions.slopeNormal.x;
            }
            else {
                velocity.y = jumpVelocity;
            }
        }
        // jump button starts in a "held" state
        jumpHeald = true;
    }

    public void OnJumpInputUp() {
        jumpHeald = false;
    }

    public Vector2 GetVelocity () {
        return velocity;
    }


    void CalculateVelocity () {
        // movement
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
        Debug.Log(targetVelocityX + " " + velocity.x);

        // gravity
        float gravityMultiplier = (!controller.collisions.below && !jumpHeald) ? minJumpGravityMultiplier : 1;
        velocity.y += gravity * GTime.deltaTime * gravityMultiplier;

        // coyote
        if (controller.collisions.below) coyoteReady = true;
        if (coyoteReady && !controller.collisions.below) {
            coyoteReady = false;
            coyoteTimer = coyoteTime;
        }
        if (coyoteTimer > 0) {
            coyoteTimer -= GTime.deltaTime;
            if (coyoteTimer < 0) coyoteTimer = 0;
        }
    }

    void HandleWallSliding() {
        // for wall climbing
        wallDirX = (controller.collisions.left)? -1 : 1;

        // wallsliding should be calculated AFTER gravity
        wallSliding = false;
        if (allowWallJump && (controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0) {
                velocityXSmoothing = 0;
                velocity.x = wallDirX * 0.01f;

                if (Mathf.Sign(directionalInput.x) != wallDirX && directionalInput.x != 0) {
                    timeToWallUnstick -= GTime.deltaTime;
                }
                else{
                    timeToWallUnstick = wallStickTime;
                }
            }
            else {
                timeToWallUnstick = wallStickTime;
            }
        } 
    }

    void DebugInstantReset() {
        //Debug.Log (controller.collisions.below);
        //if (playerInputs.Shift.WasPressed) transform.position = new Vector2(-13.55f, -1.1f); 
    }

    void HandleJumping() {
        // jump
        if (playerInputs.Space.WasPressed && directionalInput.y < 0) {
            controller.collisions.FallThoughCloud();
            return;
        }
        if (playerInputs.Space.WasPressed) OnJumpInputDown();
        if (playerInputs.Space.WasReleased) OnJumpInputUp();
        if (velocity.y < 0) jumpHeald = false;
    }

    bool FallHard() {
        // count falltime
        if (!controller.collisions.below && velocity.y <= 0) {
            fallTime += GTime.deltaTime;
            return false;
        }
        // enter land state
        if (controller.collisions.below && fallTime > longFallTime) {
            float fallPower = longFallTime/(fallTime * 1.25f);
            animate.PlayAnimation(animate.land, false, false, fallPower);
            fallTime = 0;
            return true;
        }
        // makes sure to reset fall time
        fallTime = 0;
        if (animate.IsPlaying(animate.land)) return true;
        return false;
    }

    public Bounds GetPlayerBounds() {
        return controller.colliderBox.bounds;
    }

    // GIZMOS
    void OnDrawGizmos() {
        if (Application.isPlaying) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(controller.colliderBox.bounds.center, controller.colliderBox.bounds.size);
        }
    }
}
