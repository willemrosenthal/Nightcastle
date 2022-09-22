using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingZone : MonoBehaviour
{
    public string toScene;
    public float width;
    public float height;
    Bounds bounds;

    bool inLoadingZone;

    Player player;
    GameManager gm;

    void Start() {
        gm = GameManager.Instance;
        player = Player.Instance;
        BuildBounds();
    }

    void BuildBounds() {
        bounds.size = new Vector2(width, height);
        bounds.center = (Vector2)transform.position + (Vector2)bounds.size * 0.5f;
    }

    void LateUpdate() {
        Vector2 playerCenter = player.controller.colliderBox.bounds.center;
        if (!inLoadingZone) {
            // if (bounds.Intersects(cameraBounds.bounds2D)) {
            //     EnterZone();
            // }
            if (bounds.Contains(playerCenter)) EnterZone();
        }
        else {
            if (!bounds.Contains(playerCenter)) ExitZone();
        }
    }

    void EnterZone() {
        inLoadingZone = true;
        // asyn load scene
        if (!SceneManager.GetSceneByName(toScene).isLoaded) {
            SceneManager.LoadSceneAsync(toScene, LoadSceneMode.Additive);
        } else Debug.Log("scene: " + toScene + "  is alrady loaded");
    }

    void ExitZone() {
        inLoadingZone = false;
        // if we are NOT in the scene that was loaded, unload that scene.
        if (gm.GetCurrentSceneName() != toScene) {
            SceneManager.UnloadSceneAsync(toScene);
        }
    }

    void SnapToGrid () {
        transform.position = new Vector2(Mathf.Round(transform.position.x * 4) * 0.25f, Mathf.Round(transform.position.y * 4) * 0.25f);
    } 

    void OnDrawGizmos() {
        if (!Application.isPlaying) {
            BuildBounds();
            SnapToGrid();
        }
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube (bounds.center, bounds.size);
        Gizmos.color = new Color(1,1,1,0.5f);
        Gizmos.DrawCube (bounds.center, bounds.size);
    }
}
