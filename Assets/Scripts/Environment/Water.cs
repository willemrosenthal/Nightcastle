using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

    public LayerMask collisionMask;
    public int surfacePixelSize = 1;
    Vector2 scale;
    Bounds bounds;

    float minSurfaceDist = 0.2f;
    public GameObject splashParticke;
    public GameObject splashPartickeSmall;
    public GameObject surfaceMoveParticke;

    SpriteRenderer sr;
    BoxCollider2D boxCollider2D;


    HashSet<Transform> inWater = new HashSet<Transform>();
    HashSet<Transform> onSurface = new HashSet<Transform>();
    Dictionary<Transform, Vector2> onSurfacePos = new Dictionary<Transform, Vector2>();
    Dictionary<Transform, InWater> inWaterDictionary = new Dictionary<Transform, InWater>();

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
        HashSet<Transform> onSurfaceNow = new HashSet<Transform>();
        RaycastHit2D[] surfaceObjs = Physics2D.RaycastAll(transform.position, Vector2.right, bounds.size.x, collisionMask);
        Debug.DrawRay(transform.position, Vector2.right * bounds.size.x, Color.red);
        foreach (RaycastHit2D hit in surfaceObjs) {
            onSurfaceNow.Add(hit.transform);
            if (!onSurface.Contains(hit.transform)) {
                // just entered surface
                SpawnSurfaceSplash(new Vector2(hit.transform.position.x, transform.position.y));
                onSurface.Add(hit.transform);
            }
            Vector2 point = new Vector2(hit.transform.position.x, hit.point.y);
            if (!onSurfacePos.ContainsKey(hit.transform)) {
                onSurfacePos.Add(hit.transform, point);
            }
            else {
                if (Vector2.Distance(onSurfacePos[hit.transform], point) > minSurfaceDist) {
                    onSurfacePos[hit.transform] = point;
                    Instantiate(surfaceMoveParticke, point, Quaternion.identity);
                }
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
                //if (onSurfaceNow.Contains(col.transform)) Instantiate(splashParticke, new Vector2(col.transform.position.x, transform.position.y), Quaternion.identity);
                if (col.transform == Player.Instance.transform) {
                    Debug.Log("PLAYER IN WATER!");
                }
                // adds in water component
                if (!inWaterDictionary.ContainsKey(col.transform))  {
                    inWaterDictionary.Add(col.transform, col.transform.GetComponent<InWater>());
                }
                UpdateInWater(col.transform, true);
            }
        }

        // // duplicate inWater becaseu you cant remove elements from a hashset while itterating though it
        // HashSet<Transform> inWaterCopy = new HashSet<Transform>(inWater);
        // foreach (Transform t in inWaterCopy) {
        //     // if it's gone, remove it
        //     if (!t) {
        //         inWater.Remove(t);
        //         if (onSurfacePos.ContainsKey(t)) onSurfacePos.Remove(t); // remove it from surface dictionary
        //     }
        //     // if it lives, but its not in the water
        //     else if (!inWaterNow.Contains(t)) {
        //         inWater.Remove(t); // remove it
        //         if (onSurfacePos.ContainsKey(t)) onSurfacePos.Remove(t); // remove it from surface dictionary
        //         Debug.Log(t.name +  " has left the water!");
        //         UpdateInWater(t, false); // turn off inwater
        //         // splash
        //         SpawnSurfaceSplash(new Vector2(t.position.x, transform.position.y));
        //     }
        // }
        RemoveEntriesNotInBoth(inWater, inWaterNow, "inWater");
        RemoveEntriesNotInBoth(onSurface, onSurfaceNow, "onSurface");
    }

    void RemoveEntriesNotInBoth(HashSet<Transform> removeFrom, HashSet<Transform> checkAginast, string type = "") {
        HashSet<Transform> copy = new HashSet<Transform>(removeFrom);
        foreach (Transform t in copy) {
            // if it's gone, remove it
            if (t == null) {
                removeFrom.Remove(t);
                RemoveDeadCallback(t, type);
            }
            // if it lives, but its not in the water
            else if (!checkAginast.Contains(t)) {
                removeFrom.Remove(t); // remove it
                RemoveMissingCallback(t, type);
            }
        }
    }

    void RemoveDeadCallback(Transform t, string type = "") {
        if (type == "inWater") {
            if (onSurfacePos.ContainsKey(t)) onSurfacePos.Remove(t);
        }
    }
    void RemoveMissingCallback(Transform t, string type = "") {
        if (type == "inWater") {
            if (onSurfacePos.ContainsKey(t)) onSurfacePos.Remove(t); // remove it from surface dictionary
            Debug.Log(t.name +  " has left the water!");
            UpdateInWater(t, false); // turn off inwater
            // splash
            SpawnSurfaceSplash(new Vector2(t.position.x, transform.position.y));
        }
        if (type == "onSurface") {
            SpawnSurfaceSplashSmall(new Vector2(t.position.x, transform.position.y));
        }
    }
 
    void SpawnSurfaceSplash( Vector2 point ) {
        Instantiate(splashParticke, point, Quaternion.identity);
    } 
 
    void SpawnSurfaceSplashSmall( Vector2 point ) {
        Instantiate(splashPartickeSmall, point, Quaternion.identity);
    } 

    void UpdateInWater(Transform t, bool turnON) {
        InWater w = inWaterDictionary[t];
        if (w) {
            w.inWater = turnON;
            w.water = transform;
        }
    }

}
