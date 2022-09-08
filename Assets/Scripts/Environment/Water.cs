using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

    public LayerMask collisionMask;
    public int surfacePixelSize = 1;
    Vector2 scale;
    Bounds bounds;

    public SpriteRenderer sr;
    public BoxCollider2D boxCollider2D;


    HashSet<Transform> inWater = new HashSet<Transform>();

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        ConfigureWaterShader();
    }

    void ConfigureWaterShader() {
        scale = transform.localScale;
        float pixelsTall = (sr.sprite.bounds.size.y * scale.y) * 32f;
        float topPixelPercent = (float)surfacePixelSize / pixelsTall;
        sr.material.SetFloat("_SufaceCutoff", 1-topPixelPercent);

        bounds = boxCollider2D.bounds;
    }   

    void Update() {
        // reconfigure water shader if scale changes
        if (scale.y != transform.localScale.y) {
            ConfigureWaterShader();
        }

        // use a raycastall that casts along it's entire length to get all objects touching the surface.
        RaycastHit2D[] surfaceObjs = Physics2D.RaycastAll(transform.position, Vector2.right, bounds.size.x, collisionMask);
        Debug.DrawRay(transform.position, Vector2.right * bounds.size.x, Color.red);
        foreach (RaycastHit2D hit in surfaceObjs) {
            if (hit.transform == Player.Instance.transform) {
                Debug.Log("PLAYER ON SURFACE!");
            }
        }

        // use some sort of bounds check to get all relevant objecst inside it's bounds.
        Collider2D[] collidersInWater = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0, collisionMask);
      
        // this is everything that was in the water
        HashSet<Transform> inWaterNow = new HashSet<Transform>();
        foreach (Collider2D col in collidersInWater) {
            // add eveything to the in water now hash set
            inWaterNow.Add(col.transform);

            // things that are new to the water
            if (!inWater.Contains(col.transform)) {
                Debug.Log(col.transform.name + " ENTERD THE WATER");
                inWater.Add(col.transform);
                if (col.transform == Player.Instance.transform) {
                    Debug.Log("PLAYER IN WATER!");
                }
            }
        }

        // duplicate inWater becaseu you cant remove elements from a hashset while itterating though it
        HashSet<Transform> inWaterCopy = new HashSet<Transform>(inWater);
        foreach (Transform t in inWaterCopy) {
            // if it's gone, remove it
            if (!t) {
                inWater.Remove(t);
            }
            // if it lives, but its not in the water
            else if (!inWaterNow.Contains(t)) {
                inWater.Remove(t); // remove it
                Debug.Log(t.name +  " has left the water!");
            }
        }
    }

}
