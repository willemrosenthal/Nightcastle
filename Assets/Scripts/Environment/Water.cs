using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

    public LayerMask collisionMask;
    public int surfacePixelSize = 1;
    Vector2 scale;
    Bounds bounds;

    float minSurfaceDist = 0.2f;
    public GameObject splashParticke;
    public GameObject splashPartickeSmall;
    public GameObject surfaceMoveParticke;

    SpriteRenderer sr;
    BoxCollider2D boxCollider2D;

    HashSet<Transform> inWaterObjectList = new HashSet<Transform>();
    List<WaterObject> waterObjects = new List<WaterObject>();
    //Dictionary<Transform, WaterObject> waterObjects = new Dictionary<Transform, WaterObject>();

    // HashSet<Transform> inWater = new HashSet<Transform>();
    // HashSet<Transform> onSurface = new HashSet<Transform>();
    // Dictionary<Transform, Vector2> onSurfacePos = new Dictionary<Transform, Vector2>();
    // Dictionary<Transform, InWater> inWaterDictionary = new Dictionary<Transform, InWater>();

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        ConfigureWaterShader();
    }

    void ConfigureWaterShader() {
        scale = transform.localScale;
        float pixelsTall = (sr.sprite.bounds.size.y * scale.y) * 32f;
        float topPixelPercent = (float)surfacePixelSize / pixelsTall;
        sr.material.SetFloat("_SufaceCutoff", 1-topPixelPercent);

        bounds = boxCollider2D.bounds;
    }   

    void Update() {
        // get current water level
        float waterLevel = transform.position.y;

        // reconfigure water shader if scale changes
        if (scale.y != transform.localScale.y) {
            ConfigureWaterShader();
        }

        Collider2D[] collidersInWater = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0, collisionMask);
      
        // this is everything that was in the water
        foreach (Collider2D col in collidersInWater) {
            if (!inWaterObjectList.Contains(col.transform)) {
                inWaterObjectList.Add(col.transform);
                waterObjects.Add(new WaterObject(col.transform));
            }
        }



        // itterate over water objects
        for ( int i = waterObjects.Count-1; i >= 0; i--) {

            // check if object is dead
            if (!waterObjects[i]._transform) {
                RemoveWaterObject(waterObjects[i]._transform, i);
                continue;
            }

            // get new and old bounds locations
            Bounds prevBounds = waterObjects[i]._bounds;
            waterObjects[i].Update();
            Bounds newBounds = waterObjects[i]._bounds;


            Vector2 objSurfacePt = new Vector2(newBounds.center.x, waterLevel);

            // HANDLE SPLASHES
            // if head just went below surface
            if (prevBounds.max.y >= waterLevel && newBounds.max.y < waterLevel) {
                SpawnSurfaceSplashSmall(objSurfacePt);
            }
            // // toe just entered water
            if (waterObjects[i].newInWater && prevBounds.min.y + 0.2f >= waterLevel && newBounds.min.y < waterLevel) {
                SpawnSurfaceSplash(objSurfacePt);
                waterObjects[i].newInWater = false;
            }
            // // head just surfaced
            if (prevBounds.max.y <= waterLevel && newBounds.max.y > waterLevel) {
                SpawnSurfaceSplashSmall(objSurfacePt);
            }
            // toe just left the water
            if (prevBounds.min.y <= waterLevel && newBounds.min.y > waterLevel) {
                SpawnSurfaceSplash(objSurfacePt);
            }

            // HANDLE SURFACE SPLASHES
            // if touching the surface
            if (newBounds.max.y > waterLevel && newBounds.min.y < waterLevel) {
                // if moved on the x value enough to warrent a splash
                if (waterObjects[i].xMoved > minSurfaceDist) {
                    SpawnSurfaceMoveParticle(objSurfacePt);
                    waterObjects[i].xMoved = 0;
                }
            }
            else {
                waterObjects[i].xMoved = 0;
            }

            // HANDLE ENTERING AND EXITING THE WATER
            // check if we left the water
            if (!bounds.Intersects(newBounds)) {
                // turn off in water
                if (waterObjects[i]._inWater) waterObjects[i]._inWater.inWater = false;
                // remove from list
                RemoveWaterObject(waterObjects[i]._transform, i);
                continue;
            }
            // tell the in-water component that we are in the water
            else if (waterObjects[i]._inWater) {
                waterObjects[i]._inWater.inWater = true;
                waterObjects[i]._inWater.water = transform;
            }
        }


    }
    


    void RemoveWaterObject(Transform t, int index) {
        inWaterObjectList.Remove(t);
        waterObjects.RemoveAt(index);
    }
     
    void SpawnSurfaceMoveParticle( Vector2 point ) {
        Instantiate(surfaceMoveParticke, point, Quaternion.identity);
    } 

    void SpawnSurfaceSplash( Vector2 point ) {
        Instantiate(splashParticke, point, Quaternion.identity);
    } 
 
    void SpawnSurfaceSplashSmall( Vector2 point ) {
        Instantiate(splashPartickeSmall, point, Quaternion.identity);
    } 

    class WaterObject {
        public Transform _transform;
        public BoxCollider2D _boxCollider2D;
        public InWater _inWater;
        public Controller2D _controller;
        public Bounds _bounds;
        public bool newInWater;
        public float xMoved;

        public WaterObject(Transform transform) {
            _transform = transform;
            _boxCollider2D = _transform.GetComponent<BoxCollider2D>();
            _inWater = _transform.GetComponent<InWater>();
            _controller = _transform.GetComponent<Controller2D>();
            _bounds = _boxCollider2D.bounds;
            newInWater = true;
            xMoved = 0;
        }
        
        public void Update() {
            xMoved += Mathf.Abs(_bounds.center.x - _boxCollider2D.bounds.center.x);
            _bounds = _boxCollider2D.bounds;
        }
    } 

}
