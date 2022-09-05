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

    public GameObject magicSparkle;
    public GameObject magicLine;
    GameObject magicSparkleObj;
    GameObject magicLineObj;
    Vector2 sparkleSmoothVelocity;
    Vector2 lineSmoothVelocity;
    List<Vector2> lastMagicDirection = new List<Vector2>();

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
                MakeMagicParticles();
                MoveMagicParticles(input);
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

            // kill the sparkle
            if (player.state.GetState() != "casting" && magicSparkleObj) {
                Destroy(magicSparkleObj);
                Destroy(magicLineObj);
                SpellListener.Instance.EndListening();
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

    void MakeMagicParticles() {
        if (!magicSparkleObj) {
            magicSparkleObj = Instantiate(magicSparkle, (Vector3)MagicStartPos() + transform.position, Quaternion.identity);
            magicSparkleObj.transform.parent = transform;

        }
        if (!magicLineObj) {
            magicLineObj = Instantiate(magicLine, (Vector3)MagicStartPos() + transform.position, Quaternion.identity);
            magicLineObj.transform.parent = transform;
        }
        // listen for spells
        if (!SpellListener.Instance.listening) SpellListener.Instance.StartListening();
    }
    void MoveMagicParticles(Vector2 input) {
        Vector2 basicPos = MagicStartPos();
        Vector2 goalPos = basicPos;

        if (input.y > 0 && input.x == 0) {
            goalPos += Vector2.up * 0.75f;
        }
        else if (input.x > 0 && input.y == 0) {
            goalPos += Vector2.right * 0.75f;
        }
        else if (input.y < 0 && input.x == 0) {
            goalPos += Vector2.down * 0.75f;
        }
        else if (input.x < 0 && input.y == 0) {
            goalPos += Vector2.left * 0.75f;
        }
        if ((lastMagicDirection.Count == 0 || lastMagicDirection[lastMagicDirection.Count-1] != goalPos) && goalPos != basicPos) {
            lastMagicDirection.Add(goalPos);
        }
        if (magicSparkleObj) {
            magicSparkleObj.transform.localPosition = Vector2.SmoothDamp(magicSparkleObj.transform.localPosition, goalPos, ref sparkleSmoothVelocity, 0.1f);
        }
        if (magicLineObj && lastMagicDirection.Count > 0) {
            magicLineObj.transform.localPosition = Vector2.SmoothDamp(magicLineObj.transform.localPosition, lastMagicDirection[0], ref lineSmoothVelocity, 0.1f);
            if (Vector2.Distance(magicLineObj.transform.localPosition, lastMagicDirection[0]) < 0.1f) {
                lastMagicDirection.RemoveAt(0);
            }
        }
    }

    Vector2 MagicStartPos() {
        return transform.InverseTransformPoint(player.controller.colliderBox.bounds.center) + Vector3.up * 0.25f + Vector3.left * 0.125f;
    }
}
