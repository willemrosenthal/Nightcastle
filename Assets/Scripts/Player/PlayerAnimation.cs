using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

[RequireComponent (typeof (SpriteRenderer))]
[RequireComponent (typeof (SpriteAnim))]
public class PlayerAnimation : MonoBehaviour {

    public AnimationClip attackStanding;
    public AnimationClip crouch;
    public AnimationClip fall;
    public AnimationClip idle;
    public AnimationClip jump;
    public AnimationClip land;
    public AnimationClip run;
    public AnimationClip walk;

    string current;
    Vector2 inputDirection;

    SpriteAnim anim;
    Player player;
    Controller2D controller;

    void Start() {
        player = GetComponent<Player>();
        anim = GetComponent<SpriteAnim>();
        controller = GetComponent<Controller2D>();
    }

    public void HandleAnimations(Vector2 velocity, Vector2 input) {
        inputDirection = input;

        // if airborne
        if (!controller.collisions.below) {
            if (velocity.y > 0) {
                PlayAnimation (jump);
                FaceInputDir();
            }
            else {
                PlayAnimation (fall);
                FaceInputDir();
            }
        }
        else {
            if (player.attacking) return;
            // walk
            if (input.x != 0) {
                PlayAnimation (walk);
                FaceInputDir();
            }
            else if (input.y < 0) {
                PlayAnimation (crouch);
                FaceInputDir();
            }
            else PlayAnimation (idle);
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
