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

    // ONLY USED FOR CLOCK and player-facing stuff. Interally, the game uses time since start.
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

    // checks if a given hour value (with decimal for min) has passed. I.E. 3.5 = has it been 3.5 hours since the game started?
    public static bool After (float _hours) {
        float hrs = Mathf.Floor(_hours);
        float min = Mathf.Floor((_hours - hrs) * 60);
        return (hours >= hrs) && (minutes >= min);
    }

}
