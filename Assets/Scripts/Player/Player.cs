using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (Health))]
[RequireComponent (typeof (PlayerAnimation))]
[RequireComponent (typeof (State))]
public class Player : MonoBehaviour {

    // singleton
	public static Player Instance { get; private set; }
    
    // jumping and move / walking
    public float moveSpeed = 2.5f;
    public float jumpHeight = 3.5f;
    public float timeToJumpApex = 0.4f;
    float accelerationTimeAirborne = 0.2f;
    float accelerationTimeGrounded = 0.01f;

    // gravity and jump internal variables
    float gravity;
    float jumpVelocity;

    // variable jump velocity
    public float minJumpHeight = 1;
    bool jumpHeald;
    float minJumpGravityMultiplier;

    // coyote time
    bool coyoteReady;
    float coyoteTime = 0.13f;
    float coyoteTimer;

    // sliding down slope
    bool slidingDownMaxSlope;

    // velocity
    Vector3 velocity;
    float velocityXSmoothing;

    // input
    Vector2 directionalInput;

    /// ABILITIES ///

    // wall jump
    public Vector2 wallJumpClimb = new Vector2(4f, 9f);
    public Vector2 wallJumpOff = new Vector2(4f, 5f);
    public Vector2 wallLeap = new Vector2(8f, 9f);
    float wallSlideSpeedMax = 0;//1.5f;
    float wallStickTimer;
    float wallStickTime = 3f/60f;
    float regrabTimer;
    float regrabMinTime = 0.25f;
    int wallDirX;

    // whip attack
    bool crouchedAttack;
    public GameObject whipArm;
    public GameObject whipArmCrouched;
    GameObject spawnedWhipArm;

    // casting
    int previousFacingDir;

    // fall hard landing
    float fallTime;
    float longFallTime = 0.55f;

    // run
    float runTapTime = 0.1f;
    float runTapTimer = 0;
    int runDir;

    [HideInInspector] public Controller2D controller;
    [HideInInspector] public Health health;
    [HideInInspector] public State state;
    [HideInInspector] public PlayerColliderBox playerColliderBox;
    [HideInInspector] public PlayerAnimation animate;
    [HideInInspector] public Push push;
    [HideInInspector] public PlayerAbilities abilities;

    GameManager gm;
    PlayerInputs playerInputs;
    SpriteRenderer sr;
    SpriteAnim anim;

    void Awake() {
        // singleton and if a duplicate, end code here
        Singleton();
		if (Instance != this) return;
        
        // get refs
        GetRefs();
    }

    void GetRefs() {
        gm = GameManager.Instance;
        playerInputs = gm.playerInputs;

        animate = GetComponent<PlayerAnimation>();
        health = GetComponent<Health> ();
        controller = GetComponent<Controller2D> ();
        playerColliderBox = GetComponent<PlayerColliderBox>();
        push = GetComponent<Push>();
        sr = GetComponent<SpriteRenderer>();
        state = GetComponent<State>();
        anim = GetComponent<SpriteAnim>();
        abilities = GetComponent<PlayerAbilities>();        
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
        CalculateJumpVelocity();
    }

    void CalculateJumpVelocity () {
        // calculate gravity based on desired jump height and time
        gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
        World.SetGravity(gravity);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        // calculate min jump hight diff for variable jump heights
        minJumpGravityMultiplier = jumpHeight / minJumpHeight;
    }

    void Update() {
        // things that block the player from doing things
        if (Game.IsStopped()) {
            return;
        }

        // get directional input
        directionalInput = playerInputs.Dpad;

        // invincibility flashing
        InvincibilityFlashing();

        // movement and gravity
        CalculateVelocity();

        // abilities
        HandleWallGrab();
        HandleJumping();
        HandleAttack();
        HandleHurt();
        HandleRun();
        HandleCasting();

        if (FallHard ()) {
            return;
        }

        // MOVE AND APPLY GRAVITY
        controller.Move(velocity * GTime.deltaTime);

        SlidingDownSlope();

        animate.HandleAnimations(velocity, directionalInput);
    }

    void InvincibilityFlashing () {
        if (health.IsInvincible() && state.GetState() != "hurt") {
            sr.enabled = !sr.enabled;
        } else sr.enabled = true;
    }

    void SlidingDownSlope() {
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
    }

    void HandleRun () {
        runTapTimer -= GTime.deltaTime;
        if (playerInputs.DpadRight.WasReleased) {
            runDir = 1;
            runTapTimer = runTapTime;
            if (state.CheckState("running")) state.ExitState();
        } 
        if (playerInputs.DpadLeft.WasReleased) {
            runDir = -1;
            runTapTimer = runTapTime;
            if (state.CheckState("running")) state.ExitState();
        } 
        if (runDir == 1 && playerInputs.DpadRight.IsPressed && runTapTimer > 0 && controller.collisions.below) {
            state.EnterState("running");
        }
        else if (runDir == -1 && playerInputs.DpadLeft.IsPressed && runTapTimer > 0 && controller.collisions.below) {
            state.EnterState("running");
        }
        // exit if we hit a wall
        if ((controller.collisions.right || controller.collisions.left) && state.GetState() == "running") {
            Debug.Log("should exit run");
            state.ExitState();
        }
    }

    void HandleCasting () {
        if (state.GetState() != "casting" && state.GetState() != "hurt" && playerInputs.Q.IsPressed && controller.collisions.below) {
            state.EnterState("casting");
            previousFacingDir = Mathf.FloorToInt(transform.localScale.x);
        }
        if (state.GetState() == "casting") {
            velocity.x = 0;
            if (playerInputs.Q.WasReleased) {
                state.ExitState();
                animate.FaceDir(previousFacingDir);
            }
        }
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
        if (state.CheckState("land")  || state.CheckState("hurt")) return;

        if (!state.CheckState("attack")) {
            if (playerInputs.A.WasPressed) {
                if (state.CheckState("crouch")) {
                    crouchedAttack = true;
                    animate.PlayAnimation(animate.attackCrouching);
                    playerColliderBox.SetSize("crouch");
                }
                else {
                    crouchedAttack = false;
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
                if (crouchedAttack && directionalInput.y >= 0) {
                    animate.PlayAnimation(animate.attackStanding);
                    animate.anim.SetTime(currentAnimationTime);
                    crouchedAttack = false;
                }
                else if (!crouchedAttack && directionalInput.y < 0) {
                    animate.PlayAnimation(animate.attackCrouching);
                    animate.anim.SetTime(currentAnimationTime);
                    crouchedAttack = true;
                }
            }
            if (playerInputs.A.WasReleased) {
                state.ExitSubstate();
            }
            if (!animate.IsPlaying(animate.attackStanding) && !animate.IsPlaying(animate.attackCrouching) && state.GetSubstate() != "looseWhip") {
                if (state.GetSubstate() == "hold") {
                    state.EnterSubstate("looseWhip");
                    if (crouchedAttack) {
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


    public void OnJumpInputDown() {
        Debug.Log("JUMP ATTEMPT: " + controller.collisions.below);
        // sliding
        if (state.GetState() == "wallGrab") {
            // fall off wall
            if (directionalInput.x == 0) {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
                Invoke("CoyoteWalJump", 1f/60f);
            }
            // climb jump
            else if (wallDirX == Mathf.Sign(directionalInput.x)) {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            // leap off wall
            else {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }
        // normal jump
        if (controller.collisions.below || coyoteTimer > 0) {
            coyoteTimer = 0;
            if (controller.collisions.slidingDownMaxSlope) {
                velocity.y = jumpVelocity * controller.collisions.slopeNormal.y;
                velocity.x = jumpVelocity * controller.collisions.slopeNormal.x;
            }
            else {
                velocity.y = jumpVelocity;
                // long jump
                if (state.GetState() == "running") {
                    velocity.y = jumpVelocity * 0.85f;
                }
            }
        }
        // jump button starts in a "held" state
        jumpHeald = true;
    }

    public void CoyoteWalJump() {
        if (wallStickTime > 0 && wallDirX == Mathf.Sign(directionalInput.x)) {
            velocity.x = -wallDirX * wallLeap.x;
            velocity.y = wallLeap.y;
        }
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
            if (state.GetState() == "running") runModifier = 2;
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

    void HandleWallGrab() {
        // for wall climbing
        wallDirX = (controller.collisions.left)? -1 : 1;

        // regrab timer prevets you from instantly re-grabbing the wall
        regrabTimer -= GTime.deltaTime;

        // wallsliding should be calculated AFTER gravity
        if (abilities.wallGrab && (controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0 && regrabTimer < 0) {


            // if all other conditions are met, try to wall grab
            if (state.GetState() != "wallGrab") {
                // raycast a box that's half the size of the player to a wall and see if it collides. This means he grabs based on the center of his body
                Vector2 castCenter = controller.colliderBox.bounds.center;
                Vector2 boundsSize = controller.colliderBox.bounds.size;
                boundsSize.x -= controller.GetSkinWidth() * 2;
                boundsSize.y *= 0.2f;
                castCenter.y += controller.colliderBox.bounds.size.y * 0.3f;
                float castDirX = controller.collisions.left ? -1 : 1;

                // did this as a raycast all to prevent going though solid objcts when already passing though a cloud
                RaycastHit2D[] hits = Physics2D.BoxCastAll (castCenter, boundsSize, 0, Vector2.right * castDirX, controller.GetSkinWidth(), controller.collisionMask);
                foreach (RaycastHit2D hit in hits) {
                    if (hit) {
                        if (hit.collider.tag == "Cloud" || hit.distance == 0) {
                            continue;
                        }
                        state.EnterState("wallGrab");
                        break;
                    }
                }
            }

            // if we are wall grabbing
            if (state.GetState() == "wallGrab") {
                // cancel out y velocity
                if (velocity.y < -wallSlideSpeedMax) {
                    velocity.y = -wallSlideSpeedMax;
                }
                
                // reset smoothing
                velocityXSmoothing = 0;
                velocity.x = wallDirX * 0.01f;

                // reset fall time
                fallTime = 0;

                // holding opposite dir from wall
                if (Mathf.Sign(directionalInput.x) != wallDirX && directionalInput.x != 0) {
                    wallStickTimer -= GTime.deltaTime;
                    if (wallStickTimer <= 0) {
                        velocity.x = directionalInput.x * moveSpeed;
                        state.ExitState();
                    }
                }
                else {
                    wallStickTimer = wallStickTime;
                }

                // fall down if you tap down
                if (directionalInput.y < 0) {
                    velocity.x = 0;
                    regrabTimer = regrabMinTime;
                    state.ExitState();
                }
            }
        } 
    }

    void HandleJumping() {
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
