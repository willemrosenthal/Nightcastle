using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class WorldTime {
 
    // time of day when game starts
    private static float worldTimeStart = 8.5f;

    // total elapsed seconds in world-time
    private static float elapsedWorldTime = 0;

    // hours, min, sec eleapsed in world-time
    public static float hours = 0;
    public static float minutes = 0;
    public static float seconds = 0;

    public static void UpdateTime(float deltaTime) {
        elapsedWorldTime += deltaTime;

        seconds = TimeSpan.FromSeconds(elapsedWorldTime).Seconds; //(int)(elapsedWorldTime % 60f);
        minutes = TimeSpan.FromSeconds(elapsedWorldTime).Minutes; //(int)(elapsedWorldTime / 60f) % 60;
        hours = TimeSpan.FromSeconds(elapsedWorldTime).Hours;     //(int)(elapsedWorldTime / 3600f); 
    }

    public static float GetTimeOfDayHours () {
        return Mathf.Floor(worldTimeStart + hours);
    }
    public static float GetTimeOfDayMinutes () {
        float startMinutes = Mathf.Floor((worldTimeStart - Mathf.Floor(worldTimeStart)) * 60);
        return startMinutes + minutes;
    }
    public static float GetTimeOfDaySeconds () {
        return seconds;
    }

}
