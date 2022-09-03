using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (Health))]
[RequireComponent (typeof (PlayerAnimation))]
[RequireComponent (typeof (State))]
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
    float coyoteTime = 0.13f;
    float coyoteTimer;

    // variable jump velocity
    public float minJumpHeight = 1;
    bool jumpHeald;
    float minJumpGravityMultiplier;

    // wall jump
    public bool allowWallJump = true;
    public Vector2 wallJumpClimb = new Vector2(4f, 9f);
    public Vector2 wallJumpOff = new Vector2(4f, 5f);
    public Vector2 wallLeap = new Vector2(8f, 9f);

    public float wallSlideSpeedMax = 1.5f;
    public float wallStickTime = 0.2f;
    float timeToWallUnstick;

    // whip attack
    bool crouched;
    public GameObject whipArm;
    public GameObject whipArmCrouched;
    GameObject spawnedWhipArm;

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
    [HideInInspector] public State state;
    [HideInInspector] public PlayerColliderBox playerColliderBox;
    [HideInInspector] public PlayerAnimation animate;
    [HideInInspector] public Push push;

    GameManager gm;
    PlayerInputs playerInputs;
    SpriteRenderer sr;

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
        state = GetComponent<State> ();
        health = GetComponent<Health> ();
        controller = GetComponent<Controller2D> ();
        animate = GetComponent<PlayerAnimation> ();
        playerColliderBox = GetComponent<PlayerColliderBox>();
        push = GetComponent<Push>();
        sr = GetComponent<SpriteRenderer>();
        //controller.SetDistBetweenRays(0.25f); // sets rays super close for player

        // calculate gravity based on desired jump height and time
        gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
        World.SetGravity(gravity);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        // calculate min jump hight diff for variable jump heights
        minJumpGravityMultiplier = jumpHeight / minJumpHeight;
    }

    void Update() {
        // things that block the player from doing things
        if (Dialogue.blocking) {
            return;
        }

        // get directional input
        directionalInput = playerInputs.Dpad;

        // invincibility
        if (health.IsInvincible() && state.GetState() != "hurt") {
            sr.enabled = !sr.enabled;
        } else sr.enabled = true;

        CalculateVelocity();
        HandleWallSliding();
        HandleJumping();
        HandleAttack();
        HandleHurt();
        HandleRun();
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

    float runTapTime = 0.1f;
    float runTapTimer = 0;
    int runDir;
    [HideInInspector] public bool runOk;
    [HideInInspector] public bool longJump;
    void HandleRun () {
        runTapTimer -= GTime.deltaTime;
        if (playerInputs.DpadRight.WasReleased) {
            runDir = 1;
            runTapTimer = runTapTime;
        } 
        if (playerInputs.DpadLeft.WasReleased) {
            runDir = -1;
            runTapTimer = runTapTime;
        } 
        if (runDir == 1 && playerInputs.DpadRight.IsPressed && runTapTimer > 0 && controller.collisions.below) {
            Debug.Log("RUN OK");
            runOk = true;
        }
        else if (runDir == -1 && playerInputs.DpadLeft.IsPressed && runTapTimer > 0 && controller.collisions.below) {
            Debug.Log("RUN OK");
            runOk = true;
        }
        //else if (runOk && ((runDir == -1 && playerInputs.DpadLeft.IsPressed) || (runDir == 1 && playerInputs.DpadRight.IsPressed)) && state.GetState() == "fall") runOk = true;
        else runOk = false;
    }

    void HandleHurt () {
        if (health.Histop() && state.GetState() != "hurt") {
            state.EnterState("hurt");
            velocity = new Vector2 (transform.localScale.x * -2, jumpVelocity * 0.65f);
            animate.PlayAnimation(animate.hurt);
        }
    }

    void HandleAttack () {
        // things that prevent you from attacking
        if (state.GetState() == "land" || state.GetState() == "hurt" ) return;

        if (state.GetState() != "attack") {
            if (playerInputs.A.WasPressed) {
                if (state.GetState() == "crouch") {
                    crouched = true;
                    animate.PlayAnimation(animate.attackCrouching);
                    playerColliderBox.SetSize("crouch");
                }
                else {
                    crouched = false;
                    animate.PlayAnimation(animate.attackStanding);
                }
                state.EnterState("attack");
                state.EnterSubstate("hold");
            }
        }
        else {
            // lets you switch between standing and crouch attack, early in the attack animation
            if (animate.anim.GetTime() < 0.15f && state.GetSubstate() == "hold" && (animate.IsPlaying(animate.attackStanding) || animate.IsPlaying(animate.attackCrouching))) {
                float currentAnimationTime = animate.anim.GetTime();
                if (crouched && directionalInput.y >= 0) {
                    animate.PlayAnimation(animate.attackStanding);
                    animate.anim.SetTime(currentAnimationTime);
                    crouched = false;
                }
                else if (!crouched && directionalInput.y < 0) {
                    animate.PlayAnimation(animate.attackCrouching);
                    animate.anim.SetTime(currentAnimationTime);
                    crouched = true;
                }
            }
            if (playerInputs.A.WasReleased) {
                state.ExitSubstate();
            }
            if (!animate.IsPlaying(animate.attackStanding) && !animate.IsPlaying(animate.attackCrouching) && state.GetSubstate() != "looseWhip") {
                if (state.GetSubstate() == "hold") {
                    state.EnterSubstate("looseWhip");
                    if (crouched) {
                        animate.PlayAnimation(animate.whipHoldCrouching);
                        spawnedWhipArm = Instantiate(whipArmCrouched, transform.position, Quaternion.identity);
                    }
                    else {
                        animate.PlayAnimation(animate.whipHoldStanding);
                        spawnedWhipArm = Instantiate(whipArm, transform.position, Quaternion.identity);
                    }
                    // code for entering loose whip state.
                    spawnedWhipArm.transform.parent = transform;
                    spawnedWhipArm.transform.localScale = new Vector3(1,1,1);
                }
                else {
                    state.ExitState();
                }
            }
            if (controller.collisions.below) velocity.x = 0;
            if (velocity.y > 0 && controller.collisions.below) velocity.y = 0;
        }

        // controls for loose whip state
        if (state.GetState() == "attack" && state.GetSubstate() == "looseWhip") {
            
        }

        // delete whip arm
        if (spawnedWhipArm && state.GetState() != "attack" || state.GetSubstate() != "looseWhip") Destroy(spawnedWhipArm);
         
    }

    public void SpawnWhipArm() {
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
            coyoteTimer = 0;
            if (controller.collisions.slidingDownMaxSlope) {
                velocity.y = jumpVelocity * controller.collisions.slopeNormal.y;
                velocity.x = jumpVelocity * controller.collisions.slopeNormal.x;
            }
            else {
                velocity.y = jumpVelocity;
                // long jump
                if (animate.IsPlaying(animate.run)) {
                    longJump = true;
                    velocity.y = jumpVelocity * 0.85f;
                }
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
        if (state.GetState() != "hurt") {
            float runModifier = 1;
            if (animate.IsPlaying(animate.run) || longJump) runModifier = 2;
            // movement
            float targetVelocityX = directionalInput.x * moveSpeed * runModifier;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
        }
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

    void HandleJumping() {
        // cancel long jump
        if (controller.collisions.below && longJump) {
            longJump = false;
            // lets you continue with a run after landing
            if (directionalInput.x != 0 && runDir == Mathf.Sign(directionalInput.x)) {
                runOk = true;
                runTapTimer = runTapTime;
            }
        }

        // jump
        if (playerInputs.S.WasPressed && directionalInput.y < 0) {
            controller.collisions.FallThoughCloud();
            return;
        }
        if (playerInputs.S.WasPressed) OnJumpInputDown();
        if (playerInputs.S.WasReleased) OnJumpInputUp();
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
            state.EnterState("land", animate.land.length);
            playerColliderBox.SetSize("crouch");
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
