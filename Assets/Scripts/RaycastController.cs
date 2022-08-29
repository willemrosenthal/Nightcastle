using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour {

    public LayerMask collisionMask;

    public const float skinWidth = 0.015f;
    float distBetweenRays = 0.15f;

    public void SetDistBetweenRays(float newDist) {
        distBetweenRays = newDist;
        CaclulateRaySpacing();
        UpdateRaycastOrigins();
    }

    [HideInInspector] public int horizontalRayCount;
    [HideInInspector] public int verticalRayCount;

    [HideInInspector] public float horizontalRaySpacing;
    [HideInInspector] public float verticalRaySpacing;

    [HideInInspector] public BoxCollider2D colliderBox;
    public RaycastOrigins raycastOrigins;

    public virtual void Awake() {
        colliderBox = GetComponent<BoxCollider2D> ();
    }

    public virtual void Start() {
        CaclulateRaySpacing();
    }

    public void UpdateRaycastOrigins() {
        Bounds bounds = colliderBox.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottom = new Vector2(bounds.center.x, bounds.min.y);
        raycastOrigins.top = new Vector2(bounds.center.x, bounds.max.y);
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.bounds = bounds;

        Debug.DrawLine(raycastOrigins.bottomLeft, raycastOrigins.bottomRight, new Color(1,0.5f,0,1));
        Debug.DrawLine(raycastOrigins.bottomRight, raycastOrigins.topRight, new Color(1,0.5f,0,1));
        Debug.DrawLine(raycastOrigins.topRight, raycastOrigins.topLeft, new Color(1,0.5f,0,1));
        Debug.DrawLine(raycastOrigins.topLeft, raycastOrigins.bottomLeft, new Color(1,0.5f,0,1));
    }

    public void CaclulateRaySpacing() {
        Bounds bounds = colliderBox.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        // must have at least 2 rays for each corner
        horizontalRayCount = Mathf.RoundToInt (boundsHeight/distBetweenRays);
        verticalRayCount = Mathf.RoundToInt (boundsWidth/distBetweenRays);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public struct RaycastOrigins {
        public Vector2 bottom, top;
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
        public Bounds bounds;
    }
}
