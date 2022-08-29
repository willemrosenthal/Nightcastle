using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Math {

    // plus or minus
    public static float PlusOrMinus () {
        return 1 - (Mathf.Round(Random.value) * 2);
    }

    // dynamicall ease. 0 = no easing, higher number = more easing
    public static float Ease( float x, float easingAmount = 1 ) {
        easingAmount = easingAmount+1;
        easingAmount = Mathf.Clamp(easingAmount, 1, Mathf.Infinity);
        return Mathf.Pow(x, easingAmount) / (Mathf.Pow(x, easingAmount) + Mathf.Pow(1-x, easingAmount));
    }

    // sin waves
    public static float SinWave(float amplitude, float frequency, float time = -1) {
        if (time == -1) time = GTime.time;
        return amplitude * Mathf.Sin (frequency * GTime.time);
    }

    public static float SinWaveStep(float amplitude, float frequency) {
        float prev = amplitude * Mathf.Sin (frequency * (GTime.time - GTime.deltaTimePrevious));
        float next = amplitude * Mathf.Sin (frequency * (GTime.time));
        return next - prev;
    }

    // if we dont provide graivty, it uses the world gravity
    public static Vector2 CalculateJumpVelocity(Vector2 startingPos, Vector2 endingPos, float maxHeight) {
        return CalculateJumpVelocity(startingPos, endingPos, maxHeight, World.gravity);
    }
    public static Vector2 CalculateJumpVelocity(Vector2 startingPos, Vector2 endingPos, float maxHeight, float gravity) {
        float h = maxHeight;

        float displacementY = endingPos.y - startingPos.y;
        float displacementX = endingPos.x - startingPos.x;
        //Vector2 displacementXY = new Vector2(endingPos.x -  startingPos.x, endingPos.z - startingPos.z);

        Vector2 velocityY = Vector2.up * Mathf.Sqrt(-2 * gravity * maxHeight);
        Vector2 velocityX = (Vector2.right * displacementX) / (Mathf.Sqrt(-2*maxHeight/gravity) + Mathf.Sqrt(2*(displacementY-maxHeight)/gravity));

        return velocityX + velocityY;
    }

    public static float PercentBetween(float lowEnd, float highEnd, float val) {
        return (val - lowEnd) / (highEnd - lowEnd);
    }

    public static Vector2 DegreeToVector2(float degree) {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static Vector2 RadianToVector2(float radian) {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 PointOnCircle(Vector2 _center, float _angle, float _radius) {
        return _center + new Vector2(Mathf.Sin(_angle * Mathf.Deg2Rad), Mathf.Cos(_angle* Mathf.Deg2Rad)) * _radius;
    }

    public static Vector2 RotateVector2(this Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static float AngleBetweenVector2(Vector2 vec1, Vector2 vec2) {
         Vector2 diference = vec2 - vec1;
         float sign = (vec2.y < vec1.y)? -1.0f : 1.0f;
         return Vector2.Angle(Vector2.right, diference) * sign;
    }


    // ease
    public static float EaseElasticBig (float timePercent) {
        float t = timePercent;
        float b = 0f;
        float c = 1f;
        float d = 1f;
		
        float ts = (t/=d)*t;
        float tc = ts*t;
        return b+c*(96*tc*ts + -295*ts*ts + 330*tc + -160*ts + 30*t);
    }

    public static float SlowerElastic (float timePercent) {
        float t = timePercent;
        float b = 0f;
        float c = 1f;
        float d = 1f;

        float ts=(t/=d)*t;
        float tc=ts*t;

        return b+c*(-36*tc*ts + 89*ts*ts + -67*tc + 10*ts + 5*t);
    }

    public static float Elastic2 (float timePercent) {
        float t = timePercent;
        float b = 0f;
        float c = 1f;
        float d = 1f;
        float ts=(t/=d)*t;
        float tc=ts*t;
        return b+c*(-38.4475f*tc*ts + 91.4475f*ts*ts + -67f*tc + 10f*ts + 5f*t);
    }

    public static float OutCubic (float timePercent) {
        float t = timePercent;
        float b = 0f;
        float c = 1f;
        float d = 1f;
		
        float ts =(t/=d)*t;
        float tc =ts*t;
        return b+c*(tc + -3*ts + 3*t);
    }

    public static float InCubic(float timePercent) {
        float t = timePercent;
        float b = 0f;
        float c = 1f;
        float d = 1f;

        float tc=(t/=d)*t*t;
        return b+c*(tc);
    }


    public static float InQuartic(float timePercent) {
        float t = timePercent;
        float b = 0f;
        float c = 1f;
        float d = 1f;
        float ts=(t/=d)*t;
        return b+c*(ts*ts);
    }

    public static float InQuintic (float timePercent) {
        float t = timePercent;
        float b = 0f;
        float c = 1f;
        float d = 1f;
        float ts=(t/=d)*t;
        float tc=ts*t;
        return b+c*(tc*ts);
    }

    public static float EaseInOutCubic(float timePercent) {
        return timePercent < 0.5f ? 4f * timePercent * timePercent * timePercent : 1f - Mathf.Pow(-2f * timePercent + 2f, 3f) / 2f;
    }

    public static float EaseOutBack(float timePercent) {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(timePercent - 1f, 3f) + c1 * Mathf.Pow(timePercent - 1f, 2f);
    }

    public static void Shuffle<T>(this IList<T> ts) {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
    
}
 