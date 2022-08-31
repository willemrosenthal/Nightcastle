using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitBox : MonoBehaviour {

    public LayerMask hitMask;
    public int damge = 1;
    public bool playerAttack = false;
    public GameObject hitParticle;

    BoxCollider2D colider;
    Player player;

    HashSet<Transform> hitTransforms = new HashSet<Transform>();

    void Start() {
        player = Player.Instance;
        colider = GetComponent<BoxCollider2D>();
    }

    void Update() {
        if (playerAttack) CollisionCheck();
        else PlayerCollisions();
    }

    void CollisionCheck() {
        Collider2D[] hits = Physics2D.OverlapBoxAll(colider.bounds.center, colider.bounds.size, 0, hitMask); //LayerMask.NameToLayer("Enemy")
        for (int i = 0; i < hits.Length; i++) {
            if (!hitTransforms.Contains(hits[i].transform)) {
                // record that we hit it so this collider cna only hit it once
                hitTransforms.Add(hits[i].transform);

                // get health component of whatever we hit
                Health h = hits[i].GetComponent<Health>();
                if (!h) continue; // dont bother if it doesnt have a health component

                // save it's health
                int healthWas = h.health;

                // reduce it's health
                h.LooseHealth(damge, hits[0]);

                // spawn hit particle IF we damaged it
                if (hitParticle && healthWas > h.health) {
                    Instantiate(hitParticle, colider.ClosestPoint(hits[i].bounds.center), Quaternion.identity);
                }
            }
        }
    }

    void PlayerCollisions () {
        if (Collision.Check(player, colider.bounds)) {
            player.health.LooseHealth(1);
        }
    }
}
