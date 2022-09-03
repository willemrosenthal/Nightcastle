using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersistantData {
    public static HashSet<string> deadNPCs = new HashSet<string>();

    public static void AddDeadNPC(string npcName) {
        if (!deadNPCs.Contains(npcName)) {
            deadNPCs.Add(npcName);
        }
    }
    
    public static bool CheckIfAlive (string npcName) {
        return !deadNPCs.Contains(npcName);
    }
}
