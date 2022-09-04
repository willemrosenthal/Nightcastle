using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLimiter : MonoBehaviour {

    // move this to a time class?
    public int minHours = 0; 
    [Range(0,60)] public int minMinutes = 0;
    public int maxHours = 0;
    [Range(0,60)] public int maxMinutes = 0;

    public List<string> npcsThatMustBeDead = new List<string>();
    public List<string> npcsThatMustBeAlive = new List<string>();

    // also conditions based on:
        // bosses or special enemies defeated
        // number of normal enemeis defeated, or other random game stats (map %, etc)
        // items aquired (or not aquired)

    [HideInInspector] public bool runOnRespawn = false;

    void Awake() {
        // this makes it so the first time (for the spawn object) this script is NOT run, then it runs when we try to re-sawpawn the charachter
        runOnRespawn = !runOnRespawn;
        if (runOnRespawn) {
            return;
        }


        // calculate min and max time
        float _minTime = (float)minHours + ((float)minMinutes/60f);
        float _maxTime = (float)maxHours + ((float)maxMinutes/60f);
        Debug.Log(_minTime + " " + _maxTime);

        // check if these requirements are met to allow the spawn. If any requiremetns are not met, destroy the object
        if (_minTime > 0 && !WorldTime.After(_minTime)) {
            Destroy(this.gameObject);
            return;
        }
        else if (_maxTime > 0 && WorldTime.After(_maxTime)) {
            Destroy(this.gameObject);
            return;
        }

        if (npcsThatMustBeAlive.Count > 0) {
            for (int i = 0; i < npcsThatMustBeAlive.Count; i++) {
                if (!PersistantData.CheckIfAlive(npcsThatMustBeAlive[i])) {
                    Destroy(this.gameObject);
                    return;
                }
            }
        }

        if (npcsThatMustBeDead.Count > 0) {
            for (int i = 0; i < npcsThatMustBeDead.Count; i++) {
                if (PersistantData.CheckIfAlive(npcsThatMustBeDead[i])) {
                    Destroy(this.gameObject);
                    return;
                }
            }
        }
    }

}
