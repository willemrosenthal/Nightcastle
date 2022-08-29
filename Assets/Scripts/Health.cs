using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public int maxHealth;
    public int health;
    public GameObject deathObject;

    public float looseHealthInvincibilityTime = 0;
    float invincibilityTimer = 0;

    void Start() {
        health = maxHealth;
    }

    public void LooseHealth(int amount = 1) {
        if ( IsInvincible() ) return;

        health -= amount;
        if (health < 0) health = 0;

        if (health == 0) {
            Die ();
        }
        else {
            invincibilityTimer = looseHealthInvincibilityTime;
        }
    }

    public void GainHealth(int amount = 1) {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }

    void Die () {
        Debug.Log ("death occurs");
        if (deathObject) {
            Instantiate (deathObject, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    public bool IsInvincible() {
        return invincibilityTimer > 0;
    }

    void Update() {
        invincibilityTimer -= GTime.deltaTime;
    }
}
