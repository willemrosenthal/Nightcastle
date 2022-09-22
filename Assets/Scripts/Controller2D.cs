using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller2D : RaycastController {

    public bool debug = false;

    public float maxSlopeAngle = 55;

    public CollisionInfo collisions;

    bool flatCollisionsOnTop = true;

    // this is turned on to prevent self-collision
    bool useRacastAll = false;

    // gravity direction
    float gravityDir = 1;

    InWater inWater;

    public override void Start() {
        base.Start();

        CheckForSelfCollision();

        // get inWater
        inWater = GetComponent<InWater>();
    }

    void CheckForSelfCollision () {
        // if this object's layer is found in it's collision layermask, turn on useRaycastAll;
        if (( collisionMask & (1 << gameObject.layer)) != 0) {
            useRacastAll = true;
        }
    }

    void WaterMovement(ref Vector2 moveAmount) {
        if (inWater && inWater.inWater) {
            moveAmount.x *= inWater.dampenMovementX;
            moveAmount.y *= inWater.dampenMovementY;
        }
    }

    public void Move(Vector2 moveAmount, bool standingOnPlatform = false) {     
        // in water
        WaterMovement(ref moveAmount);

        UpdateRaycastOrigins ();
        collisions.Reset ();
        collisions.moveAmountOld = moveAmount;
        gravityDir = 1;

        // reverse gravity
        if (transform.localScale.y < 0) {
            gravityDir = -1;
            moveAmount.y *= -1;
            ReverseGravityOrigins();
        }

        
        if (moveAmount.x != 0) { // handles clouds
            SolidBlockHorizontalCollisions(ref moveAmount);
            HorizontalCollisions (ref moveAmount);
            HandleClimbSlope(ref moveAmount);
        }

        //above collisions should be done before below in case moveAmount.y is reversed durring this process
        if ((gravityDir > 0 && moveAmount.y > 0) || (gravityDir < 0 && moveAmount.y < 0)) {
            AboveCollisions (ref moveAmount);
            AboveEdgeCollisions (ref moveAmount);
        }

        // below collisions
        if ((gravityDir > 0 && moveAmount.y < 0) || (gravityDir < 0 && moveAmount.y > 0)) {
            BelowCollisions (ref moveAmount);
            BelowEdgeCollisions (ref moveAmount);
        }

        //above collisions AGAIN
        if ((gravityDir > 0 && moveAmount.y > 0) || (gravityDir < 0 && moveAmount.y < 0)) {
            AboveCollisions (ref moveAmount);
            AboveEdgeCollisions (ref moveAmount);
        }

        transform.Translate (moveAmount);

        if (standingOnPlatform) {
            collisions.below = true;
        }
    }

    void SolidBlockHorizontalCollisions(ref Vector2 moveAmount) {
        float directionX = Mathf.Sign (moveAmount.x);
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
        // version using collider.Raycast
        Vector2 castCenter = raycastOrigins.bounds.center;
        Vector2 boundsSize = raycastOrigins.bounds.size;
    
        // if airborne, do full box cast otherwise do a cast, but ignore the lower 25% of the charachter
        if (collisions.wasGrounded) {
            castCenter += Dir(Vector2.up) * raycastOrigins.bounds.size.y * 0.25f * 0.5f;
            boundsSize = raycastOrigins.bounds.size;
            boundsSize.y *=  0.75f;

            // dont bother doing this check if the bounds is too small
            if (boundsSize.y < 0.75f) return;
            
            
            /* // debug, draw collider
            Bounds b = new Bounds (castCenter, boundsSize);
            Debug.DrawLine(new Vector2(b.min.x, b.min.y), new Vector2(b.max.x, b.min.y), new Color(0.5f,1f,0,1));
            Debug.DrawLine(new Vector2(b.max.x, b.min.y), new Vector2(b.max.x, b.max.y), new Color(0.5f,1f,0,1));
            Debug.DrawLine(new Vector2(b.max.x, b.max.y), new Vector2(b.min.x, b.max.y), new Color(0.5f,1f,0,1));
            Debug.DrawLine(new Vector2(b.min.x, b.max.y), new Vector2(b.min.x, b.min.y), new Color(0.5f,1f,0,1));
            */
        }
        
        // did this as a raycast all to prevent going though solid objcts when already passing though a cloud
        RaycastHit2D[] hits = Physics2D.BoxCastAll (castCenter, boundsSize, 0, Vector2.right * directionX, rayLength, collisionMask);
        foreach (RaycastHit2D hit in hits) {
            if (hit) {
                if (hit.collider.tag == "Cloud" || hit.distance == 0) {
                    continue;
                }
                //float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

                //if (slopeAngle > maxSlopeAngle) {
                moveAmount.x = (hit.distance - skinWidth * 1.01f) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
                collisions.sideCollisionObject = hit.transform;
                //}
            }
        }
    }

    void HorizontalCollisions (ref Vector2 moveAmount) {
        float directionX = Mathf.Sign(moveAmount.x);
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;


        // look for wall collisions
        for (int i = 0; i < horizontalRayCount; i++) {
            Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
            rayOrigin += Dir(Vector2.up) * (horizontalRaySpacing * i);
            RaycastHit2D hit = Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit) {
                if (hit.collider.tag == "Cloud") {
                    continue;
                }
                if (hit.distance == 0) {
                    continue;
                }
                float slopeAngle = Vector2.Angle (hit.normal, Dir(Vector2.up));

                if (slopeAngle > maxSlopeAngle) {
                    moveAmount.x = (hit.distance - skinWidth * 1.01f) * directionX;
                    rayLength = hit.distance;

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                    collisions.sideCollisionObject = hit.transform;
                }
            }
        }
    }
    
    void HandleClimbSlope (ref Vector2 moveAmount) {
        float directionX = Mathf.Sign(moveAmount.x);
        float rayLength = Mathf.Abs(moveAmount.x);

        // FIRST:
        // racast from bottom (inset by skin) in x move dir. if slope is hit, move to slope, handle climbing
        RaycastHit2D hit = Raycast(raycastOrigins.bottom, Vector2.right * directionX, rayLength, collisionMask);

        if (hit) {
            float slopeAngle = Vector2.Angle (hit.normal, Dir(Vector2.up));
            if (hit.distance == 0 || (hit.collider.tag == "Cloud" && slopeAngle > maxSlopeAngle)) return; // NEW: (hit.collider.tag == "Cloud" && slopeAngle > maxSlopeAngle) <-- to let you climb cloud slopes
            // climbing slope
            if (slopeAngle > 0 && slopeAngle <= maxSlopeAngle) {
                float distToSlopeStart = 0;
                if (slopeAngle != collisions.slopeAngleOld) {
                    distToSlopeStart = hit.distance - skinWidth;
                    moveAmount.x -= distToSlopeStart * directionX;
                    //Debug.Log("happened");
                    //Debug.Break();
                }
                ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                moveAmount.x += distToSlopeStart * directionX;

                // make sure we are skin width above the surface
                Vector2 newRayOrigin = raycastOrigins.bottom + Vector2.right * moveAmount.x + Dir(Vector2.up) * moveAmount.y;
                RaycastHit2D newHit = Raycast(newRayOrigin, Dir(Vector2.down), skinWidth, collisionMask);
                if (newHit) {
                    float yDiff = newHit.distance - skinWidth;
                    moveAmount.y += -yDiff * gravityDir;
                }
            }
        }
    }

    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal) {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        Vector2 preClimbMove = moveAmount;

        // if we are not jumping on a slope, climb the slope
        //if (moveAmount.y <= 0) {
        moveAmount.y = climbmoveAmountY * gravityDir;
        moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            

        // dont cancel out vertical movement
        if ((preClimbMove.y > moveAmount.y && gravityDir > 0) || (preClimbMove.y < moveAmount.y && gravityDir < 0)) {
            moveAmount.y = preClimbMove.y;
        }
        else {
            collisions.below = true;
            collisions.onSlope = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
        //}
    }

    void AboveCollisions (ref Vector2 moveAmount) {
        Vector2 savedMove = moveAmount;
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        // cast ray down from where we will be after having moved
        Vector2 rayOrigin = raycastOrigins.top + Vector2.right * moveAmount.x;// + Vector2.up * moveAmount.y;

        // shoot ray down from bottom center
        RaycastHit2D hit = Raycast(rayOrigin, Dir(Vector2.up), rayLength, collisionMask);

        if (hit && hit.collider.tag != "Cloud") {
            moveAmount.y = (hit.distance - skinWidth);
            collisions.above = true;
            collisions.aboveHitNormal = hit.normal;
            
            if (collisions.below) {
                moveAmount.x = 0;
            }
        }
    }

    void AboveEdgeCollisions (ref Vector2 moveAmount) {
        Vector2 savedMove = moveAmount;
        // handle flat cliffs and downward sloping cliffs
        //if (oldMoveAmount == moveAmount) {
        RaycastHit2D hitLeft =  Raycast(raycastOrigins.topLeft  + Vector2.left * skinWidth + Vector2.right * moveAmount.x,  Dir(Vector2.up), Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        AboveEdge(hitLeft, ref moveAmount);
        RaycastHit2D hitRight = Raycast(raycastOrigins.topRight + Vector2.right * skinWidth + Vector2.right * moveAmount.x, Dir(Vector2.up), Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
        AboveEdge(hitRight, ref moveAmount);
        //}
    }

    void AboveEdge (RaycastHit2D hit, ref Vector2 moveAmount) {
        if (hit && hit.collider.tag != "Cloud") {
            bool useFlatCollisions = true;

            if (!flatCollisionsOnTop) {
                DrawPoint(hit.point, Color.cyan, 0.1f, 1);
                // get longest possible collidable slope
                float adj = (raycastOrigins.bounds.size.x * 0.5f) + skinWidth;
                float opp = Mathf.Tan(maxSlopeAngle * Mathf.Deg2Rad) * adj;

                Vector2 rayOrigin = new Vector2(raycastOrigins.top.x + moveAmount.x, hit.point.y);
                // see if there is a slope above you
                RaycastHit2D slopeCheck = Raycast(rayOrigin, Dir(Vector2.up), opp + skinWidth, collisionMask);
                Debug.DrawRay(raycastOrigins.top, Dir(Vector2.up) * (opp + skinWidth), Color.red);

                // if we find a slope above the player's head, we *may* ignore the flat collisions
                if (slopeCheck) {
                    // we hit a slope, so maybe don't use the flat collision we got
                    useFlatCollisions = false;

                    DrawPoint(slopeCheck.point, Color.yellow, 0.1f, 1);

                    // check if the flat collision has a vertical edge
                    // castsa a few rays to look for drop offs that could be faking out the detection
                    for (int i = 0; i < 3; i++) {
                        Vector2 newRayOrigin = new Vector2 (raycastOrigins.top.x + moveAmount.x, Mathf.Lerp(hit.point.y, slopeCheck.point.y , 0.25f * (float)(i + 1)));
                        float newRayLength = (raycastOrigins.bounds.size.x * 0.5f) + skinWidth;

                        RaycastHit2D wallChecker = Raycast(newRayOrigin, Vector2.right * -Mathf.Sign(hit.normal.x), newRayLength, collisionMask);
                        if (wallChecker) {
                            float wallAngle = Vector2.Angle (-wallChecker.normal, Dir(Vector2.up));
                            DrawPoint(wallChecker.point, Color.black, 0.05f);

                            if (wallAngle >= maxSlopeAngle) {
                                useFlatCollisions = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (useFlatCollisions) {
                moveAmount.y = hit.distance - skinWidth;
                collisions.above = true;
                collisions.aboveHitNormal = hit.normal;
                if (collisions.below) {
                    moveAmount.x = 0;
                }
            }
        }
    }

    void BelowCollisions (ref Vector2 moveAmount) {

        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        // if grounded and not trying to jump, then double ray length to help keep snapped to ground
        if ((collisions.wasGrounded || collisions.below) && collisions.moveAmountOld.y < 0) {
            rayLength += skinWidth * 5;
        }

        // if we were grounded, and we still have downard force...
        // increase ray based on how much x movement there is to insure that it can detect the steepest climbalbe sope downward
        if (collisions.wasGrounded && Mathf.Abs(moveAmount.x) > 0) {
            float longerRay = (Mathf.Abs(moveAmount.x) / Mathf.Tan((90-maxSlopeAngle) * Mathf.Deg2Rad)) + skinWidth;
            if (longerRay > rayLength) {
                rayLength = longerRay;
            }
        }

        // cast ray down from where we will be after having moved
        Vector2 rayOrigin = raycastOrigins.bottom + Vector2.right * moveAmount.x;// + Vector2.up * moveAmount.y;

        // shoot ray down from bottom center
        RaycastHit2D hit = Raycast(rayOrigin, Dir(Vector2.down), rayLength, collisionMask);

        if (hit) {
            // dont collide with cloud if inside the collider
            if ((hit.distance == 0 || collisions.fallThoughCloudOk > 0) && hit.collider.tag == "Cloud") return;

            float slopeAngle = Vector2.Angle (hit.normal, Dir(Vector2.up));

            if (slopeAngle <= maxSlopeAngle) {
                //float wasY = moveAmount.y;
                // only set new y dist if it brings you down farther
                //if ((hit.distance - skinWidth) * -1 < moveAmount.y) {
                moveAmount.y = (hit.distance - skinWidth) * -1 * gravityDir;
                //}
                collisions.below = true;
                if (collisions.above) {
                    moveAmount.x = 0;
                }
                // remain on sope
                if (slopeAngle != 0) {
                    collisions.onSlope = true;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void BelowEdgeCollisions (ref Vector2 moveAmount) {
        // FOR DEBUG, TURN THIS ON
        //GTime.timeScale = 0.1f;

        // we only need to check this if we are NOT on a slope.
        if (collisions.onSlope) return;

        // so we can see if it was changed
        Vector2 oldMoveAmount = moveAmount;

        // handle walking off upward sloping cliff
        if (collisions.wasGrounded && !collisions.below) {
            Vector2 rayOrigin = raycastOrigins.bottom + Dir(Vector2.down) * (Mathf.Abs(moveAmount.y) + skinWidth) + Vector2.right * moveAmount.x;
            float rayLength = (raycastOrigins.bounds.size.x * 0.5f) + skinWidth;

            RaycastHit2D cliffEdgeCheck = Raycast(rayOrigin, Vector2.right * -1, rayLength, collisionMask);
            WalkOffClipEdgeHit(cliffEdgeCheck, ref moveAmount);
            
            if (!collisions.below) cliffEdgeCheck = Raycast(rayOrigin, Vector2.right * 1, rayLength, collisionMask);
            WalkOffClipEdgeHit(cliffEdgeCheck, ref moveAmount);
        }

        // falling version
        if (!collisions.wasGrounded && !collisions.below) {
            Vector2 rayOrigin = raycastOrigins.bottom + Dir(Vector2.down) * skinWidth + Vector2.right * moveAmount.x;
            float rayLength = (raycastOrigins.bounds.size.x * 0.5f) + skinWidth;

            // bump away from walls
            RaycastHit2D bumpOffWallCheck = Raycast(rayOrigin, Vector2.right * -1, rayLength, collisionMask);
            BumpAwayFromWall (bumpOffWallCheck, ref moveAmount);
            bumpOffWallCheck = Raycast(rayOrigin, Vector2.right * 1, rayLength, collisionMask);
            BumpAwayFromWall (bumpOffWallCheck, ref moveAmount);

            // recalulate ray origin
            rayOrigin = raycastOrigins.bottom + Dir(Vector2.down) * (Mathf.Abs(moveAmount.y) + skinWidth) + Vector2.right * moveAmount.x;
            DrawPoint(rayOrigin, Color.cyan, 0.05f, 3);

            RaycastHit2D cliffEdgeCheck = Raycast(rayOrigin, Vector2.right * -1, rayLength, collisionMask);
            FallOffCliffEdgeHit(cliffEdgeCheck, ref moveAmount);
            if (!collisions.below) cliffEdgeCheck = Raycast(rayOrigin, Vector2.right * 1, rayLength, collisionMask);
            FallOffCliffEdgeHit(cliffEdgeCheck, ref moveAmount);
        }

        // handle flat cliffs and downward sloping cliffs
        if (oldMoveAmount == moveAmount) {
            RaycastHit2D hitLeft =  Raycast(raycastOrigins.bottomLeft  + Vector2.left * skinWidth,  Dir(Vector2.down), Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
            RaycastHit2D hitRight = Raycast(raycastOrigins.bottomRight + Vector2.right * skinWidth, Dir(Vector2.down), Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);

            if (hitLeft || hitRight) {
                BelowEdge(hitLeft, ref moveAmount);
                BelowEdge(hitRight, ref moveAmount);
            }
        }
    }

    void WalkOffClipEdgeHit(RaycastHit2D hit, ref Vector2 moveAmount) {
        // if no hit, end here
        if (!hit) return;

        // dont collide with cloud if inside the collider
        if ((hit.distance == 0 || collisions.fallThoughCloudOk > 0) && hit.collider.tag == "Cloud") return;

        float slopeAngle = Vector2.Angle (hit.normal, Dir(Vector2.up));

        if (slopeAngle >= maxSlopeAngle) { 
            Vector2 rayOrigin = new Vector2(hit.point.x, raycastOrigins.bottom.y);
            float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;
            //DrawPoint(rayOrigin, Color.cyan, 0.05f, 0);

            //Debug.DrawRay(rayOrigin, Vector2.down * (Mathf.Abs(moveAmount.y) + downCheck), Color.yellow);
            RaycastHit2D edgeHit = Raycast(rayOrigin, Dir(Vector2.down), rayLength, collisionMask);
            if (edgeHit) {
                moveAmount.y = -edgeHit.distance + skinWidth;
                collisions.below = true;
                //DrawPoint(edgeHit.point, Color.yellow, 0.02f, 0);
                //Debug.DrawLine(edgeHit.point + Vector2.right * 0.1f, edgeHit.point + Vector2.right * 0.1f + Vector2.up * moveAmount.y, Color.yellow, 3);
            }
            // failsafe?
            else {
                collisions.below = true;
                moveAmount.y = 0;
                if (collisions.above) {
                    moveAmount.x = 0;
                }
            }
        }
    }

    void BumpAwayFromWall (RaycastHit2D hit, ref Vector2 moveAmount) {
        if (!hit || hit.distance == 0) return; // dont do anything if inside a collider

        float slopeAngle = Vector2.Angle (hit.normal, Dir(Vector2.up));
        if (slopeAngle >= maxSlopeAngle) {
            if (hit.distance > raycastOrigins.bounds.size.x * 0.5f) moveAmount.x += Mathf.Sign(hit.normal.x) * skinWidth * 0.01f;
        }
    }

    void FallOffCliffEdgeHit(RaycastHit2D hit, ref Vector2 moveAmount) {
        if (!hit) return;
        if (hit.collider.tag == "Cloud") return; // dont collide with cloud

        float slopeAngle = Vector2.Angle (hit.normal, Dir(Vector2.up));

        //Debug.Log("hit 1");
        //DrawPoint(hit.point, Color.blue, 0.02f, 3);
        if (slopeAngle >= maxSlopeAngle) {
            // next we cast a ray downward from the player at that x value to get the actual fall distance
            //DrawPoint(cliffEdgeCheck.point, Color.black, 0.01f);
            Vector2 rayOrigin = new Vector2(hit.point.x, raycastOrigins.bottom.y - skinWidth);
            //DrawPoint(rayOrigin, Color.cyan, 0.05f, 3);

            //Debug.DrawRay(rayOrigin, Vector2.down * Mathf.Abs(moveAmount.y), Color.yellow);
            RaycastHit2D edgeHit = Raycast(rayOrigin, Dir(Vector2.down), Mathf.Abs(moveAmount.y), collisionMask);
            if (edgeHit) {
                moveAmount.y = -edgeHit.distance;
                collisions.below = true;
                //DrawPoint(edgeHit.point, Color.yellow, 0.05f, 3);
            }
        }
    }

    void BelowEdge (RaycastHit2D hit, ref Vector2 moveAmount) {
        // SO FAR THIS ONLY HANDLES DOWNWARD SLOPING EDGES
        if (hit) {
            // dont collide with cloud if inside the collider
            if ((hit.distance == 0 || collisions.fallThoughCloudOk > 0) && hit.collider.tag == "Cloud") return;
            // if not grounded and hit is inside continue
            if (hit.distance == 0 && !collisions.wasGrounded) return;
            
            float slopeAngle = Vector2.Angle (hit.normal, Dir(Vector2.up));
            if (slopeAngle > 0 && slopeAngle < maxSlopeAngle) {
                // adjasent side = dist from hit corner to center of collider
                float adj = (raycastOrigins.bounds.size.x * 0.5f) + skinWidth;
                float opp = Mathf.Tan(maxSlopeAngle * Mathf.Deg2Rad) * adj;

                // added additional skin width to be super safe
                RaycastHit2D slopeCheck = Raycast(raycastOrigins.bottom, Dir(Vector2.down), opp + skinWidth * 2, collisionMask);
                //Debug.DrawRay(raycastOrigins.bottom, Vector2.down * (opp + skinWidth * 2), Color.cyan, 1);
                bool sideWallsDetected = false;

                if (slopeCheck) {
                    bool _edgeDebugs = false;
                    // sanity check. Fire rays from 1/4, 1/2 and 3/4 between [hit point] and [slopeCheck hit point]
                    // fire them in the Mathf.Sign(hit.normal.x) direction. Ray Length = raycastBounds.size * 0.5 + skinWidth.
                    // if any rays hit a surface with a slope >= maxSlope then cancel the "slope".
                    // this stops you from falling through a corner
                    for (int i = 0; i < 3; i++) {
                        Vector2 _checkPt = new Vector2 (raycastOrigins.bottom.x, Mathf.Lerp(slopeCheck.point.y, hit.point.y, 0.25f * (float)(i + 1)));
                        float _rayLength = (raycastOrigins.bounds.size.x * 0.5f) + skinWidth;
                        if (_edgeDebugs) DrawPoint(_checkPt, Color.green, 0.02f);

                        RaycastHit2D wallChecker = Raycast(_checkPt, Vector2.right * -Mathf.Sign(hit.normal.x), _rayLength, collisionMask);
                        if (wallChecker) {
                            if (_edgeDebugs) DrawPoint(wallChecker.point, Color.black, 0.05f);
                            float wallAngle = Vector2.Angle (wallChecker.normal, Dir(Vector2.up));
                            if (wallAngle >= maxSlopeAngle) {
                                sideWallsDetected = true;
                                break;
                            }
                        }
                    }
                }
                if (slopeCheck && !sideWallsDetected) {
                    DrawPoint(slopeCheck.point, Color.white, 0.1f);

                    slopeAngle = Vector2.Angle (slopeCheck.normal, Dir(Vector2.up));
                    collisions.onSlope = true;
                    return;
                }
                else {
                    slopeAngle = 0;
                    collisions.onSlope = false;
                }
            }
            if (slopeAngle == 0 && !collisions.onSlope) {
                moveAmount.y = (hit.distance>skinWidth) ? (hit.distance - skinWidth) * -1 * gravityDir : 0;
                collisions.below = true;
            }
        }
    }

    Vector2 Dir (Vector2 dir) {
        dir.y *= gravityDir;
        return dir;
    }

    void DrawPoint(Vector2 point, Color color, float size = 0.3f, float time = 0.5f) {
        Debug.DrawLine(point + Vector2.left * size * 0.5f, point + Vector2.right * size * 0.5f, color, time);
        Debug.DrawLine(point + Vector2.up * size * 0.5f, point + Vector2.down * size * 0.5f, color, time);
    }

    public RaycastHit2D Raycast(Vector2 rayOrigin, Vector2 rayDir, float rayLength, LayerMask _collisionMask) {
        if (useRacastAll) {
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, rayDir, rayLength, _collisionMask);
            for (int i = 0; i < hits.Length; i++) {
                if (hits[i].collider != colliderBox) return hits[i];
            }
            return new RaycastHit2D();
        }
        else {
            return Physics2D.Raycast(rayOrigin, rayDir, rayLength, _collisionMask);
        }
    }

    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;

        public bool wasGrounded;

        public bool onSlope;
        public bool climbingSlope; // do i need?
        public bool descendingSlope; // do i need?
        public bool slidingDownMaxSlope;

        public Vector2 aboveHitNormal;

        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public Vector2 moveAmountOld;
        public int faceDir;
        public bool fallingThroughPlatform;

        public int fallThoughCloudOk;

        public Transform sideCollisionObject;

        public void FallThoughCloud() {
            fallThoughCloudOk = 10; // number of frames to allow falling though a cloud
        }

        public void Reset() {
            wasGrounded = below;

            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector2.zero;
            onSlope = false;

            aboveHitNormal = Vector2.zero;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;

            sideCollisionObject = null;

            if (fallThoughCloudOk > 0) fallThoughCloudOk--;
        }
    }
}
