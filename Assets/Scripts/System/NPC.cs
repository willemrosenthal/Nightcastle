using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

public class NPC : MonoBehaviour
{
    [TextArea] public string text = "";
    bool talking;

    float talkRange = 1;

    DialogueText dialogue;

    public AnimationClip idle;
    public AnimationClip talk;
    public AnimationClip walk;
    public AnimationClip hit;

    PlayerInputs playerInputs;
    Player player; 
    SpriteAnim anim;

    void Start() {
        anim = GetComponent<SpriteAnim>(); 
        player = Player.Instance;
        playerInputs = GameManager.Instance.playerInputs;
    }

    void Update() {
        // dont excute if far away
        if (Vector2.Distance(transform.position, Camera.main.transform.position) > 100) return;

        float distToPlayer = Vector2.Distance(player.transform.position, transform.position);
        if (!talking) {
            if (player.controller.collisions.below && playerInputs.D.WasPressed && distToPlayer < 1 && Dialogue.GetDialogue() == null) {
                dialogue = Dialogue.NewDialogue(text);
                talking = true; 
            }
        } 
        if (talking) {
            if (distToPlayer > talkRange * 1.1f) dialogue.Interrupt();
            if (dialogue == null) talking = false;
        }
    }

    void OnDestroy() {
        if (talking) {
            dialogue.Interrupt();
        }
    }
}
