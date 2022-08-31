using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMan : MonoBehaviour {

	public static CameraMan Instance { get; private set; }

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
			//DontDestroyOnLoad(gameObject);
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
    }

    void LateUpdate() {
        // centers cameraman on player's collider
        transform.position = player.controller.colliderBox.bounds.center;

        // update the camera after the cameraman has done his thing
        cameraFollow.CameraUpdate();
    }
}
