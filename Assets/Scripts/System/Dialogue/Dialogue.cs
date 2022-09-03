using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Dialogue {

    public static bool blocking;
    public static DialogueText dialogue;

    public static DialogueText NewDialogue (string textToSay, bool blocking = true) {
        if (DialogueText.Instance != null) {
            EndDialogue();
            //Debug.Log("dialouge already in progress");
            //return null;
        }
        if (blocking) blocking = true;

        GameObject newDialogue = new GameObject();
        dialogue = newDialogue.AddComponent<DialogueText>();
        dialogue.Initilize(textToSay, blocking);
        return dialogue;
    }

    public static DialogueText GetDialogue() {
        return dialogue;
    }

    public static void InterruptDialogue() {
        dialogue.Interrupt();
    }

    public static void EndDialogue() {
        dialogue.EndDialogue();
    }

    public static void Block() {
        blocking = true;
    }
    public static void Unblock() {
        blocking = false;
    }

}
