using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour {

    // Controller2D controller2D;

    // Dictionary<int, ColissionData> collisionDictionary = new Dictionary<int, ColissionData>();

    // void Start () {
    //     controller2D = GetComponent<Controller2D>();
    // }

    // public ColissionData Check (MonoBehaviour obj) {
    //     ColissionData c = null;

    //     int key = obj.GetInstanceID();

    //     if (collisionDictionary.ContainsKey(key)) c = collisionDictionary[key];
    //     else {
    //         c = new ColissionData();
    //         c.justHit = true;
    //         c.hitTime = GTime.time;
    //     }
    //     c.ex

    //     //Debug.Log(obj.GetType());
    //     //if (Collision.Check(controller2D, (obj.GetType()) obj))
    //     Debug.Log(obj.name + " collide: " + Collision.Check(controller2D, obj));

    //     return c;
    // }

    // public class ColissionData {
    //     public bool justHit;
    //     public float hitTime;
    // }
}
