using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;


[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (SpriteAnim))]
[RequireComponent (typeof (Health))]
public class NPC : MonoBehaviour {
    // data for tracking NPC
    public string npc = "Jimmy";

    [TextArea] public string text = "";
    [TextArea] public string hitText = "";
    bool talking;

    public bool facePlayer;

    float talkRange = 1;

    DialogueText dialogue;

    float hitTime = 0.75f;
    float hitTimer;
    int currentHealth;
    

    public AnimationClip idle;
    public AnimationClip talk;
    public AnimationClip walk;
    public AnimationClip hit;

    PlayerInputs playerInputs;
    Player player; 
    SpriteAnim anim;
    Health health;
    Animator animator;

    void Awake() {
        CheckIfALive();
    }

    void CheckIfALive() {
        if (!PersistantData.CheckIfAlive(npc)) {
            Destroy(this.gameObject);
        }
    }

    void Start() {
        health = GetComponent<Health>();
        currentHealth = health.health;
        anim = GetComponent<SpriteAnim>(); 
        animator = GetComponent<Animator>();
        player = Player.Instance;
        playerInputs = GameManager.Instance.playerInputs;

        if (anim && idle) anim.Play(idle);

        if (facePlayer) FacePlayer();
    }

    void Update() {
        // dont excute if far away
        if (Vector2.Distance(transform.position, Camera.main.transform.position) > 100) return;

        float distToPlayer = Vector2.Distance(player.transform.position, transform.position);
        if (!talking) {
            // interract with npc
            if (player.controller.collisions.below && playerInputs.D.WasPressed && distToPlayer < 1 && Dialogue.GetDialogue() == null) {
                dialogue = Dialogue.NewDialogue(text);
                talking = true; 
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                anim.Play(talk);
            }
            // hit
            if (currentHealth != health.health) {
                currentHealth = health.health;
                dialogue = Dialogue.NewDialogue(hitText, false);
                talking = true;
                if (hit) anim.Play(hit);
                hitTimer = hitTime;
            }
        } 
        else if (talking) {
            hitTimer -= GTime.deltaTime;
            if (distToPlayer > talkRange * 1.1f && hitTimer <= 0) dialogue.Interrupt();
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
        if (health && health.health <= 0) {
            // add to list of dead NPCS
            PersistantData.AddDeadNPC(npc);
            if (talking) {
                dialogue.Interrupt();
            }
        }
    }
}
