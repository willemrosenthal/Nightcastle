using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

[RequireComponent (typeof (SpriteRenderer))]
[RequireComponent (typeof (SpriteAnim))]
public class PlayerAnimation : MonoBehaviour {

    public AnimationClip attackCrouching;
    public AnimationClip attackStanding;
    public AnimationClip crouch;
    public AnimationClip fall;
    public AnimationClip hurt;
    public AnimationClip idle;
    public AnimationClip jump;
    public AnimationClip jumpLong;
    public AnimationClip land;
    public AnimationClip push;
    public AnimationClip run;
    public AnimationClip walk;
    public AnimationClip whipHoldCrouching;
    public AnimationClip whipHoldStanding;

    public AnimationClip casting_down;
    public AnimationClip casting_left;
    public AnimationClip casting_neutral;
    public AnimationClip casting_right;
    public AnimationClip casting_up;

    string current;
    Vector2 inputDirection;

    [HideInInspector] public SpriteAnim anim;
    Player player;
    Controller2D controller;
    State state;

    void Start() {
        player = GetComponent<Player>();
        anim = GetComponent<SpriteAnim>();
        controller = GetComponent<Controller2D>();
        state = GetComponent<State>();
    }

    public void HandleAnimations(Vector2 velocity, Vector2 input) {
        inputDirection = input;

        // if airborne
        if (!controller.collisions.below) {
            // while being hurt, do nothing
            if (state.GetState() == "hurt") return;
            // while you are attacking, only do that
            if (state.GetState() == "attack") return;

            // jump
            if (velocity.y > 0) {
                player.state.EnterState("jump");
                if (player.longJump) PlayAnimation(jumpLong);
                else PlayAnimation (jump);
                FaceInputDir();
            }
            // fall
            else {
                player.state.EnterState("fall");
                PlayAnimation (fall);
                FaceInputDir();
            }
        }
        else {
            // while you are attacking, only do that
            if (state.GetState() == "attack") return;

            // casting
            if (player.state.GetState() == "casting") {
                if (input.y > 0 && input.x == 0) PlayAnimation(casting_up);
                else if (input.x > 0 && input.y == 0) PlayAnimation(casting_right);
                else if (input.y < 0 && input.x == 0) PlayAnimation(casting_down);
                else if (input.x < 0 && input.y == 0) PlayAnimation(casting_left);
                else PlayAnimation(casting_neutral);
                FaceDir(1);
                // should remember previous facing and return there when done
            }
            // walk
            else if (input.x != 0) {
                if (player.push.pushing) PlayAnimation (push);
                else if ((player.runOk || IsPlaying(run)) && !player.controller.collisions.left && !player.controller.collisions.right) PlayAnimation(run);
                else PlayAnimation (walk);
                player.state.EnterState("walk");
                FaceInputDir();
            }
            // crouch
            else if (input.y < 0) {
                player.state.EnterState("crouch");
                player.playerColliderBox.SetSize("crouch");
                PlayAnimation (crouch);
                FaceInputDir();
            }
            // idle
            else {
                player.state.EnterState("idle");
                PlayAnimation (idle);
            }
        }
    }

    void FaceInputDir() {
        FaceDir(Mathf.RoundToInt(inputDirection.x));
    }

    public void PlayAnimation ( AnimationClip clip, bool overrideStoppedCurrent = false, bool overrideSame = false, float speed = 1 ) {
        if ((current != clip.name || overrideStoppedCurrent) && !overrideSame && !anim.IsPlaying(clip)) {
            Play(clip);
        }
        else if (overrideSame) {
            Play(clip);
        }
    }

    void Play (AnimationClip clip) {
        anim.Play(clip);
        current = clip.name;
    }

    public bool IsPlaying( AnimationClip clip ) {
        return anim.IsPlaying(clip);
    }

    public void FaceDir( int dir ) {
        dir = Mathf.Clamp(dir, -1, 1);
        if (dir == 0) return;
        transform.localScale = new Vector3 ( Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y, transform.localScale.z);
    }

    public float GetFacing() {
        return Mathf.Sign(transform.localScale.x);
    }
}
