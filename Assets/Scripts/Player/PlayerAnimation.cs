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
    public AnimationClip land;
    public AnimationClip push;
    public AnimationClip run;
    public AnimationClip walk;
    public AnimationClip whipHoldCrouching;
    public AnimationClip whipHoldStanding;

    string current;
    Vector2 inputDirection;

    SpriteAnim anim;
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
                PlayAnimation (jump);
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

            // walk
            if (input.x != 0) {
                if (player.push.pushing) PlayAnimation (push);
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
