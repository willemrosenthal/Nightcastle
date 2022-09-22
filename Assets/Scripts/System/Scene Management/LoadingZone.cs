using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingZone : MonoBehaviour
{
    public Transform levelContent;
    public string toScene;
    public string id;
    public float width;
    public float height;
    Bounds bounds;

    bool inLoadingZone;

    Player player;
    GameManager gm;
    LoadingZoneConnector zoneConnector;

    void Awake() {
        if (levelContent == null) levelContent = transform.parent;

        // get zone connector
        zoneConnector = LoadingZoneConnector.Instance;
        ConnectZones();
    }

    void Start() {
        // get the zone connector again since the first one loaded may not have an instance
        zoneConnector = LoadingZoneConnector.Instance;
        gm = GameManager.Instance;
        player = Player.Instance;
        BuildBounds();
    }

    void BuildBounds() {
        bounds.size = new Vector2(Mathf.Abs(width), height);
        bounds.center = (Vector2)transform.position + new Vector2(width, height) * 0.5f;
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

    void ConnectZones() {
        if (zoneConnector.triggeredID == id && zoneConnector.triggeredScene == gameObject.scene.name) {
            Vector2 spaceDiff = zoneConnector.triggredZone.position - transform.position;
            levelContent.transform.position += (Vector3)spaceDiff;
        }
    }

    void EnterZone() {
        inLoadingZone = true;
        // asyn load scene
        if (!SceneManager.GetSceneByName(toScene).isLoaded) {
            zoneConnector.triggeredID = id;
            zoneConnector.triggeredScene = toScene;
            zoneConnector.triggredZone = transform;
            SceneManager.LoadSceneAsync(toScene, LoadSceneMode.Additive);
        }
        else {
            Debug.Log("scene: " + toScene + "  is alrady loaded");
        }
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
