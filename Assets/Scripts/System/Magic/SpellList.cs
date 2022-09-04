using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellList {
    public static Dictionary<string, string> spells = new Dictionary<string, string>();

    public static void Initilize() {
        AddSpell("^•v•^•v", "become mouse");
        AddSpell("<^>•<^>", "heal");
        AddSpell("<^>", "small heal");
        AddSpell("^v", "reverse gravity");
        //AddSpell("^•v•^•v", "become mouse again?");
    }

    public static void AddSpell(string code, string spellName) {
        if (spells.ContainsKey (code)) {
            Debug.LogError("spell dictionary already conaints spell: " + code + " " + spellName);
        }
        else {
            spells.Add(code, spellName);
        }
    }

    public static string GetSpell(string code) {
        string _spell = "none";
        if (spells.ContainsKey(code)) _spell = spells[code];
        if (_spell == "reverse gravity") ReverseGravity(); // not a good way to do this
        return _spell;
    } 

    public static void ReverseGravity() {
        // reverse gravity
        Player.Instance.transform.localScale = new Vector3(Player.Instance.transform.localScale.x, Player.Instance.transform.localScale.y *-1, Player.Instance.transform.localScale.z);
        Player.Instance.transform.localPosition += Vector3.down * Player.Instance.transform.localScale.y * Player.Instance.controller.colliderBox.size.y;
    }
}
