using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

public class Attack : MonoBehaviour {

    // attack hitbox
    public GameObject attackBox;

    // length of time hitbox is active
    public float attackTime;

    GameObject currentAttack;
    bool attackStarted;
    float attackTimer;

    // spawns attackbox
    public void AttackBox() {
        if (currentAttack) Destroy(currentAttack);
        currentAttack = Instantiate(attackBox, transform.position, Quaternion.identity);
        currentAttack.transform.parent = transform;
        currentAttack.transform.localPosition = Vector3.zero;
        currentAttack.transform.localScale = new Vector3(1,1,1);
        attackTimer = attackTime;
    }

    void Update() {
        // if we are in an attack
        if (attackTimer > 0) {
            attackTimer -= GTime.deltaTime;

            if (attackTimer < 0) {
                attackTimer = 0;
                Destroy(currentAttack);
            }
        }
    }
}
