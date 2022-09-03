using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;


[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (SpriteAnim))]
[RequireComponent (typeof (Health))]
public class NPC : MonoBehaviour
{
    [TextArea] public string text = "";
    bool talking;

    public bool facePlayer;

    float talkRange = 1;

    DialogueText dialogue;

    public AnimationClip idle;
    public AnimationClip talk;
    public AnimationClip walk;

    PlayerInputs playerInputs;
    Player player; 
    SpriteAnim anim;
    Health health;
    Animator animator;

    void Start() {
        health = GetComponent<Health>();
        anim = GetComponent<SpriteAnim>(); 
        animator = GetComponent<Animator>();
        player = Player.Instance;
        playerInputs = GameManager.Instance.playerInputs;

        if (anim && idle) anim.Play(idle);
    }

    void Update() {
        // dont excute if far away
        if (Vector2.Distance(transform.position, Camera.main.transform.position) > 100) return;

        float distToPlayer = Vector2.Distance(player.transform.position, transform.position);
        if (!talking) {
            if (player.controller.collisions.below && playerInputs.D.WasPressed && distToPlayer < 1 && Dialogue.GetDialogue() == null) {
                dialogue = Dialogue.NewDialogue(text);
                talking = true; 
                if (anim && talk) {
                    animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                    anim.Play(talk);
                }
            }
        } 
        if (talking) {
            if (distToPlayer > talkRange * 1.1f) dialogue.Interrupt();
            if (dialogue == null) talking = false;

            // if talking ends
            if (talking == false) {
                animator.updateMode = AnimatorUpdateMode.Normal;
                anim.Play(idle);
            }
        }

        if (facePlayer) FacePlayer();
    }



    public void FacePlayer() {
        FaceDir( (int) Mathf.Sign(player.transform.position.x - transform.position.x) );
    }

    public void FaceDir( int dir ) {
        dir = Mathf.Clamp(dir, -1, 1);
        if (dir == 0) return;
        transform.localScale = new Vector3 ( Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y, transform.localScale.z);
    }

    void OnDestroy() {
        // if it dies from being killed rather than despawning, then do death talk
        if (health.health <= 0) {
            if (talking) {
                dialogue.Interrupt();
            }
        }
    }
}
