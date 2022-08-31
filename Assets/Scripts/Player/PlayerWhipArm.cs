using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWhipArm : MonoBehaviour {

    public Sprite neutral;
    public Sprite fw;
    public Sprite fwup;
    public Sprite up;
    public Sprite backup;
    public Sprite back;
    public Sprite backdown;
    public Sprite down;
    public Sprite fwdown;

    public Vector2[] whipAttachPts;

    [Range(0,8)] public int currentWhipPt = 0;

    public Transform whipRope;

    int facing;

    PlayerInputs playerInputs;
    SpriteRenderer sr;
    Player player;

    void Awake() {
        player = Player.Instance;
        whipRope.GetComponent<Rope>().ropeBuildDir = new Vector2(player.transform.localScale.x, 0);
    }

    void Start() {
        playerInputs = GameManager.Instance.playerInputs;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update() {
        facing = (int)player.transform.localScale.x;

        Vector2 directionalInput = playerInputs.Dpad;
        directionalInput.x *= facing;

        // round in case its not an int
        directionalInput.x = Mathf.Round(directionalInput.x);
        directionalInput.y = Mathf.Round(directionalInput.y);

        // select correct sprite
        if      (directionalInput.x > 0 && directionalInput.y == 0) {
            sr.sprite = fw;
            currentWhipPt = 1;
        }
        else if (directionalInput.x > 0 && directionalInput.y > 0) {
            sr.sprite = fwup;
            currentWhipPt = 2;
        }
        else if (directionalInput.x == 0 && directionalInput.y > 0) {
             sr.sprite = up;
            currentWhipPt = 3;
        }
        else if (directionalInput.x < 0 && directionalInput.y > 0) {
             sr.sprite = backup;
            currentWhipPt = 4;
        }
        else if (directionalInput.x < 0 && directionalInput.y == 0) {
             sr.sprite = back;
            currentWhipPt = 5;
        }
        else if (directionalInput.x < 0 && directionalInput.y < 0) {
             sr.sprite = backdown;
            currentWhipPt = 6;
        }
        else if (directionalInput.x == 0 && directionalInput.y < 0) {
             sr.sprite = down;
            currentWhipPt = 7;
        }
        else if (directionalInput.x > 0 && directionalInput.y < 0) {
             sr.sprite = fwdown;
            currentWhipPt = 8;
        }
        else  {
            sr.sprite = neutral;
            currentWhipPt = 0;
        }

        Vector2 whipPt = whipAttachPts[currentWhipPt];
        whipPt.x *= facing;
        whipRope.transform.position = (Vector2)transform.position + whipPt;
    }

    void OnDrawGizmos() {
        if (!Application.isPlaying) {
            if (!sr) sr = GetComponent<SpriteRenderer>();
            Vector2 pt = whipAttachPts[currentWhipPt];
            Gizmos.DrawLine((Vector2)transform.position + pt + Vector2.up * 0.1f,(Vector2)transform.position + pt + Vector2.down * 0.1f);
            Gizmos.DrawLine((Vector2)transform.position + pt + Vector2.left * 0.1f,(Vector2)transform.position + pt + Vector2.right * 0.1f);
            if (currentWhipPt == 0) sr.sprite = neutral;
            else if (currentWhipPt == 1) sr.sprite = fw;
            else if (currentWhipPt == 2) sr.sprite = fwup;
            else if (currentWhipPt == 3) sr.sprite = up;
            else if (currentWhipPt == 4) sr.sprite = backup;
            else if (currentWhipPt == 5) sr.sprite = back;
            else if (currentWhipPt == 6) sr.sprite = backdown;
            else if (currentWhipPt == 7) sr.sprite = down;
            else if (currentWhipPt == 8) sr.sprite = fwdown;
        }
    }
}
