using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMan : MonoBehaviour {

	public static CameraMan Instance { get; private set; }

    Vector2 playerCameraOffset;

    CameraFollow cameraFollow;
    Player player;

    void Awake() {
        // set up as singleton, 
        Singleton();
        // if a duplicate, end code here
		if (Instance != this) return;
    }

    void Singleton () {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy (gameObject);
		}
	}

    void Start() {
        player = Player.Instance;
        // get camerafollow
        cameraFollow = GameManager.Instance.cameraFollow;
        // set this to the target
        cameraFollow.target = transform;

        // get player bounds offset to center camera on middle of charachter rather than feet
        playerCameraOffset = player.GetComponent<BoxCollider2D>().bounds.center - player.transform.position;
    }

    void LateUpdate() {
        // centers cameraman on player's collider
        transform.position = (Vector2)player.transform.position + playerCameraOffset * player.transform.localScale.y;

        // update the camera after the cameraman has done his thing
        cameraFollow.CameraUpdate();
    }
}
