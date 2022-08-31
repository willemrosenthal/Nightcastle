using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public int maxHealth;
    public int health;
    public GameObject damageHitPartcile;
    public GameObject deathObject;

    public float looseHealthInvincibilityTime =  0.1f;
    public float histopTime = 0.33f;
    float invincibilityTimer = 0;
    float hitstopTimer = 0;

    void Start() {
        health = maxHealth;
        if (histopTime <= 0) histopTime = looseHealthInvincibilityTime;
    }

    public void LooseHealth(int amount = 1, Collider2D col = null) {
        if ( IsInvincible() ) return;

        health -= amount;
        if (health < 0) health = 0;

        // hit damage particle
        if (damageHitPartcile) {
            Vector2 spawnPt = col ? col.bounds.center : transform.position;
            Transform t = Instantiate (damageHitPartcile, transform.position, Quaternion.identity).transform;
            t.localScale = transform.localScale; // keeps facing dir
        }

        if (health == 0) {
            Die ();
        }
        else {
            invincibilityTimer = looseHealthInvincibilityTime;
            hitstopTimer = histopTime;
        }
    }

    public void GainHealth(int amount = 1) {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }

    void Die () {
        if (this.tag != "Player") {
            if (deathObject) {
                Transform t = Instantiate (deathObject, transform.position, Quaternion.identity).transform;
                t.localScale = transform.localScale; // keeps facing dir
            }
            Destroy(this.gameObject);
        }
    }

    public bool IsInvincible() {
        return invincibilityTimer > 0;
    }

    public bool Histop() {
        return hitstopTimer > 0;
    }

    void Update() {
        invincibilityTimer -= GTime.deltaTime;
        hitstopTimer -= GTime.deltaTime;
    }
}
