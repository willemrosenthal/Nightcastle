using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

public class DeathObject : MonoBehaviour {

    public Vector2 shake = Vector2.zero;
    public AnimationClip clip;
    public float life;
    public bool dieWhenEmpty;
    public bool clipLengthIsLife;
    public bool fickerWhenDying;
    public GameObject[] deathObjects;
    public AudioClip sound; // NOT WORKING YET! <-- need sound mananger

    float timer;
    Vector2 originalPos;
    int shakeDir = 1;

    SpriteRenderer sr;
    SpriteAnim anim;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        if (clipLengthIsLife) {
            life = clip.length;
        }
        originalPos = transform.localPosition;

        // convert shake to pixels
        if (shake != Vector2.zero) {
            shake *=  (1f/32f);
        }

        // add this to current zone objects
        GameManager.Instance.GetCurrentZone().AddToZoneObjects(this.gameObject);
    }

    void Update() {
        // shake
        if (shake != Vector2.zero) {
            transform.localPosition = originalPos + Vector2.right * shake.x * shakeDir;
            transform.localPosition += Vector3.up * shake.y * shakeDir;
            shakeDir *= -1;
        }
        // die when children are dead
        if (dieWhenEmpty) {
            if (transform.childCount == 0) Die();
        }
        // otherwise die when timer elapses
        else {
            timer += GTime.deltaTime;
            if (fickerWhenDying &&  timer > life * 0.75f) sr.enabled = !sr.enabled; 
            if (timer >= life) Die();
        }
    }

    void Die() {
        if (deathObjects.Length > 0) {
            if (shake.x != 0) transform.localPosition = new Vector3(originalPos.x, transform.localPosition.y);
            if (shake.y != 0) transform.localPosition = new Vector3(transform.localPosition.y, originalPos.y);

            for (int i = 0; i < deathObjects.Length; i++) {
                Transform t = Instantiate(deathObjects[i], transform.position, Quaternion.identity).transform;
                // keeps facing dir
                t.localScale = transform.localScale;
            }
        }
        Destroy(this.gameObject);
    }
}
