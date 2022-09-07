using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderBox : MonoBehaviour
{
    public Vector2 normal = new Vector2(0.4f, 1.3125f);
    public Vector2 jumping = new Vector2(0.4f, 1.1f);
    public Vector2 crouch = new Vector2(0.4f, 0.75f);
    public Vector2 mouse = new Vector2(0.3f, 0.3f);

    BoxCollider2D boxCollider2D;
    Player player;

    void Start() {
        player = Player.Instance;
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public void SetSize(string size = "normal") {
        Vector2 newSize = Vector2.zero;

        if (size == "normal") newSize = normal ;
        else if (size == "crouch") newSize = crouch;
        else if (size == "mouse") newSize = mouse;

        // failsafe if unrecongized string
        else { 
            newSize = normal; 
            Debug.LogError("unrecognized size string: '" + size + "' in PlayerColliderBox.SetSize");
        }

        boxCollider2D.size = newSize;
        boxCollider2D.offset = new Vector2(0, boxCollider2D.size.y * 0.5f);
    }

    void Update() {
        // when to remain in crouch size
        if (player.state.GetState() != "crouch" &&
            player.state.GetState() != "land" &&
            player.state.GetState() != "attack" &&
            !player.animate.IsPlaying(player.animate.attackCrouching) &&
            !player.animate.IsPlaying(player.animate.whipHoldCrouching)
            ){
            SetSize();
        }
    }
}
