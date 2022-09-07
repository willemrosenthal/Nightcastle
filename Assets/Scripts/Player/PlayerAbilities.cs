using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour {
    public bool doubleJump;
    public bool floatJump;
    public bool wallGrab;
    public bool superRun;
    public bool bretheUnderwater; // perminant unlock from spell
    public bool walkOnWater; // temporary, has a timer? or is screen based? or toggle? <<<< COULD BE RUN ON WATER! if walking, you fall, if running you can stay on top!
    public bool reverseGravity; // toggle state from spell
    public bool mouse; // turn into a mouse (or rat) - toggle from spell
    public bool senseEvil; // could be an item
    public bool telepathy; // toggle that affects talking
}
