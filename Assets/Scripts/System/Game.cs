using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Game {

    public static bool paused;
    public static bool inDialogue;
    public static bool inCutscene;
    public static bool inMenu;

    public static bool IsStopped() {
        return paused || inDialogue || inCutscene || inMenu;
    }

    public static void Pause() {
        paused = true;
    }
    public static void UnPause() {
        paused = false;
    }

    public static void InDialogue() {
        inDialogue = true;
    }
    public static void EndDialogue() {
        inDialogue = false;
    }

    public static void InCutscene() {
        inCutscene = true;
    }
    public static void EndCutscene() {
        inCutscene = false;
    }

    public static void InMenu() {
        inMenu = true;
    }
    public static void ExitMenu() {
        inMenu = false;
    }
}
