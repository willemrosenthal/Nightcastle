using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour {

    public LayerMask spawnObjectLayers;

    public float width;
    public float height;
    Bounds bounds;

    float camHeight = 6.5f;
    float camWidht = 10f;

    bool currentZone;
    
    Vector2 cacheLocation = new Vector3(10000,10000,10000);
	List<SpawnObject> zoneObjectCache = new List<SpawnObject>();
    List<GameObject> currentZoneObjects = new List<GameObject>();

    CameraBounds cameraBounds;
    CameraFollow cameraFollow;
    Player player;

    void Start() {
        player = Player.Instance;
        cameraBounds = Camera.main.GetComponent<CameraBounds>();
        cameraFollow = Camera.main.GetComponent<CameraFollow>();

        BuildBounds();
        FindSpawnObjects();
    }

    void BuildBounds() {
        bounds.size = new Vector2(width, height);
        bounds.center = (Vector2)transform.position;
    }

    void FindSpawnObjects() {
        Collider2D[] spawnObjectsFound = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0, spawnObjectLayers); //LayerMask.NameToLayer("Enemy")
        Debug.Log("FOUND SPAWN OBJECTS: " + spawnObjectsFound.Length);

        // add objects that spawn in the zone to the zoneObjectCache
        // this also moves them away and deactivaets them if they are Enemies.
        for (int i = 0; i < spawnObjectsFound.Length; i++) {
            Debug.Log(spawnObjectsFound[i].gameObject.name);
            zoneObjectCache.Add(new SpawnObject( spawnObjectsFound[i].gameObject, cacheLocation )); // gets the prefab ref
        }
    }

    void Update() {
        // check if camera is inside the bounds
        if (!currentZone) {
            Vector2 playerCenter = player.controller.colliderBox.bounds.center;
            // if (bounds.Intersects(cameraBounds.bounds2D)) {
            //     EnterZone();
            // }
            if (bounds.Contains(playerCenter)) EnterZone();
        }
        else {
            // do current zone shit?
        }
    }

    public Bounds GetBounds() {
        return bounds;
    }

    void EnterZone() {
        Debug.Log("ENTERED NEW ZONE: " + this.name);
        currentZone = true;
        // update camera room bounds
        cameraFollow.SetZoneBounds(this);

        // spawn monsters
        SpawnObjects();
    }

    public void ExitZone () {
        currentZone = false;
        // desapwn monsters
        DestroyObjects();
    }

    void SpawnObjects() {
        currentZoneObjects = new List<GameObject>();
        for (int i = 0; i < zoneObjectCache.Count; i++) {
            // enable enemy if it is an enemy
            if (zoneObjectCache[i].enemy) zoneObjectCache[i].enemy.enabled = true;
            GameObject obj = Instantiate(zoneObjectCache[i].obj, zoneObjectCache[i].spawnPoint, Quaternion.identity );
            // re-disable the cached enemy (if it is an enemy)
            if (zoneObjectCache[i].enemy) zoneObjectCache[i].enemy.enabled = false;
            // add object to current zone objects
            currentZoneObjects.Add(obj);
        }
    }

    void DestroyObjects() {
        for (int i = 0; i < currentZoneObjects.Count; i++) {
            Destroy(currentZoneObjects[i]);
        }
    }

    void OnDrawGizmos() {
        if (!Application.isPlaying) {
            BuildBounds();
            ConstrainBoundsToCam ();
            SnapToGrid();
        }
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    void ConstrainBoundsToCam () {
        if (bounds.size.y < camHeight) {
            bounds.size = new Vector2 (bounds.size.x, camHeight);
            height = camHeight;
        }
        if (bounds.size.x < camWidht) {
            bounds.size = new Vector2 (camWidht, bounds.size.y);
            width = camWidht;
        }
    }

    void SnapToGrid () {
        transform.position = new Vector2(Mathf.Round(transform.position.x * 4) * 0.25f, Mathf.Round(transform.position.y * 4) * 0.25f);
    } 

    struct SpawnObject {
        public SpawnObject (GameObject _obj, Vector2 _cacheLocation) {
            obj = _obj;
            enemy = obj.GetComponent<Enemy>();
            spawnPoint = obj.transform.position;
            // disable enemy
            if (enemy) enemy.enabled = false;
            // move original object to cache location
            _obj.transform.position = _cacheLocation;
        }
        public GameObject obj;
        public Enemy enemy;
        public Vector2 spawnPoint;
    }
}
