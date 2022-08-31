using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour {

    public string state = "normal";
    public string substate = "none";

    float timer;
    float exitStateTime;

    float substateTimer;
    float exitSubstateTime;

    void Start () {
        if (state == "") state = "normal";
        if (substate == "") state = "none";
    }

    void Update() {
        timer += GTime.deltaTime;
        if (exitStateTime > 0 && timer > exitStateTime) {
            ExitState();
        }
        if (exitSubstateTime > 0 && substateTimer > exitSubstateTime) {
            ExitSubstate();
        }
    }

    // STATE
    public void EnterState(string _state, float exitAfter = -1) {
        state = _state;
        exitStateTime = exitAfter; 
        timer = 0;
    }
    public string GetState() {
        return state;
    }
    public void ExitState() {
        EnterState("normal");
    }

    // SUBSTATE
    public void EnterSubstate(string _subState, float exitAfter = -1) {
        substate = _subState;
        exitSubstateTime = exitAfter; 
        substateTimer = 0;
    }
    public string GetSubstate() {
        return substate;
    }
    public void ExitSubstate() {
        EnterSubstate("none");
    }


    // // exit state when condition is met
    // public void EndWhen(ref T value, ref T condition, string switchToState = "normal") {
    //     StartCoroutine(EndIfConditonMet(ref value, ref condition, switchToState));
    // }

    // IEnumerator EndIfConditonMet(ref T value, ref T condition, string switchToState = "normal") {
    //     while (value != condition) {
         
    //       yield return null;
    //     }
    // }
}
