using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour {

    public string zoneName = "";
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
    GameManager gm;

    void Start() {
        gm = GameManager.Instance;
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
        // add objects that spawn in the zone to the zoneObjectCache
        // this also moves them away and deactivaets them if they are Enemies.
        for (int i = 0; i < spawnObjectsFound.Length; i++) {
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
        currentZone = true;
        // exit any zone we are currently in
        if (gm.GetCurrentZone()) gm.GetCurrentZone().ExitZone();
        // set current zone in gm
        gm.SetCurrentZone(this);
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
            GameObject obj = Instantiate(zoneObjectCache[i].obj, zoneObjectCache[i].spawnPoint, Quaternion.identity, transform.parent );
            // re-disable the cached enemy (if it is an enemy)
            if (zoneObjectCache[i].enemy) zoneObjectCache[i].enemy.enabled = false;
            // add object to current zone objects
            currentZoneObjects.Add(obj);
        }
    }

    public void AddToZoneObjects(GameObject obj) {
        currentZoneObjects.Add(obj);
    }

    void DestroyObjects() {
        for (int i = 0; i < currentZoneObjects.Count; i++) {
            Destroy(currentZoneObjects[i]);
        }
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

    [HideInInspector] public Color zoneColor;
    void OnDrawGizmos() {

        if (!Application.isPlaying) {
            GetZoneColor();
            BuildBounds();
            ConstrainBoundsToCam ();
            SnapToGrid();
        }

        Gizmos.color = zoneColor;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        
        Gizmos.DrawWireSphere(bounds.min, 0.75f);
        Gizmos.DrawWireSphere(new Vector2(bounds.min.x, bounds.max.y), 0.75f);
        Gizmos.DrawWireSphere(bounds.max, 0.75f);
        Gizmos.DrawWireSphere(new Vector2(bounds.max.x, bounds.min.y), 0.75f);

        Gizmos.DrawSphere(bounds.min, 0.3f);
        Gizmos.DrawSphere(new Vector2(bounds.min.x, bounds.max.y), 0.3f);
        Gizmos.DrawSphere(bounds.max, 0.3f);
        Gizmos.DrawSphere(new Vector2(bounds.max.x, bounds.min.y), 0.3f);
    }

    void OnDrawGizmosSelected() {   
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(bounds.center, bounds.size + Vector3.one * 0.02f);

        Gizmos.DrawWireSphere(bounds.min, 0.76f);
        Gizmos.DrawWireSphere(new Vector2(bounds.min.x, bounds.max.y), 0.76f);
        Gizmos.DrawWireSphere(bounds.max, 0.76f);
        Gizmos.DrawWireSphere(new Vector2(bounds.max.x, bounds.min.y), 0.76f);

        Gizmos.DrawSphere(bounds.min, 0.20f);
        Gizmos.DrawSphere(new Vector2(bounds.min.x, bounds.max.y), 0.20f);
        Gizmos.DrawSphere(bounds.max, 0.20f);
        Gizmos.DrawSphere(new Vector2(bounds.max.x, bounds.min.y), 0.20f);
    }

    void GetZoneColor() {
        Random.InitState(transform.GetInstanceID());
        zoneColor = new Color(Random.value, Random.value, Random.value);
        //gets a more intense random color
        float zeroOutVal = Random.value;
        if (Random.value < 0.33) {
            zoneColor.r *= 0.25f;
            if (Random.value < 0.5f) zoneColor.g *= 2;
            else zoneColor.b *= 2;
        }
        else if (Random.value < 0.66) {
            zoneColor.g *= 0.25f;
            if (Random.value < 0.5f) zoneColor.r *= 2;
            else zoneColor.b *= 2;
        }
        else {
            zoneColor.b *= 0.25f;
            if (Random.value < 0.5f) zoneColor.r *= 2;
            else zoneColor.g *= 2;
        }

    }

}
