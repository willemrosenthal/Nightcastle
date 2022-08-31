using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Collision {
    
    // mystery
    public static bool Check(Controller2D a, MonoBehaviour b, bool make2D = true) {
        if (b.GetType() == typeof (Enemy)) return Check(a, (Enemy)b, make2D);
        else if (b.GetType() == typeof (Player)) return Check(a, (Player)b, make2D);
        else if (b.GetType() == typeof (Controller2D)) return Check(a, (Controller2D)b, make2D);
        else return false;
    }

    // controller 2Ds
    public static bool Check(Controller2D a, Controller2D b, bool make2D = true) {
        return Check(a.colliderBox.bounds, b.colliderBox.bounds, make2D);
    }

    // player and enemy
    public static bool Check(Player a, Enemy b, bool make2D = true) {
        return Check(a.GetPlayerBounds(), b.controller.colliderBox.bounds, make2D);
    }
    // enemy and player
    public static bool Check(Enemy a, Player b, bool make2D = true) {
        return Check(b.GetPlayerBounds(), a.controller.colliderBox.bounds, make2D);
    }
    // player and controller 2D
    public static bool Check(Player a, Controller2D b, bool make2D = true) {
        return Check(a.GetPlayerBounds(), b.colliderBox.bounds, make2D);
    }
    // player and controller 2D
    public static bool Check(Player a, Bounds b, bool make2D = true) {
        return Check(a.GetPlayerBounds(), b, make2D);
    }
    // controller 2D and player
    public static bool Check(Controller2D a, Player b, bool make2D = true) {
        return Check(b.GetPlayerBounds(), a.colliderBox.bounds, make2D);
    }
    
    // controller 2D and enemy
    public static bool Check(Controller2D a, Enemy b, bool make2D = true) {
        return Check(b.controller.colliderBox.bounds, a.colliderBox.bounds, make2D);
    }
    // enemy and controller 2D 
    public static bool Check(Enemy a, Controller2D b, bool make2D = true) {
        return Check(a.controller.colliderBox.bounds, b.colliderBox.bounds, make2D);
    }

    // base
    public static bool Check(Bounds a, Bounds b, bool make2D = true) {
        if (make2D) {
            a.center = (Vector2) a.center;
            b.center = (Vector2) b.center;
        }
        return a.Intersects(b);
    }
}
