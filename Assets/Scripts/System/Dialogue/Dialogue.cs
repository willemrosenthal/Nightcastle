using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Dialogue {

    public static bool inDialogue;
    public static DialogueText dialogue;

    public static DialogueText NewDialogue (string textToSay) {
        if (DialogueText.Instance != null) return null;

        GameObject newDialogue = new GameObject();
        dialogue = newDialogue.AddComponent<DialogueText>();
        dialogue.Initilize(textToSay);
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

}
