using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {

    public bool darwWithLineRenderer = false;
    public Material lineMaterial;
    public Sprite segmentSprite;
    public Sprite endSprite;
    public GameObject endOject;
    public string sortLayer = "Gameplay";
    public int startingSort = 0;
    public bool drawSegmentSprites = true;
    public bool rotateSegSprites = false;
    public float spriteRotOffset = 0;
    [Range(0,50)]public int simulatonAccuracy = 10;
    [Range(0,50)]public int totalSegments = 7;
    [Range(0.01f,0.5f)]public float segmentDistance = 0.5f;

    //public float volitility = 1;
    public Vector2 gravity = new Vector2(0f, -1f);

    public bool attachEndOneToMouse = false;
    public Transform endOneAttach;
    public Transform endTwoAttach;

    private LineRenderer lineRenderer;
    [HideInInspector] public List<RopeSegment> ropeSegments = new List<RopeSegment>();
    [HideInInspector] public float ropeSegLen = 0.25f;
    [HideInInspector] public int numSegments = 20;

    [HideInInspector] public bool active = true;

    List <Transform> segmentSprites = new List<Transform>();

    bool firstAcive = true;

    public Vector2 ropeBuildDir = new Vector2(1,0);
    public float initialStretch = 1;

    public virtual void Start() {
        // set up line renderer
        if (darwWithLineRenderer) {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
        }

        BuildRope();
        if (drawSegmentSprites) MakeSegmentSprites();
    }

    void BuildRope() {
        ropeSegLen = segmentDistance;
        numSegments = totalSegments;
        Vector2 ropeStartPoint = transform.position;
        for (int i = 0; i < numSegments; i++) {
            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y += ropeSegLen * ropeBuildDir.y * initialStretch;
            ropeStartPoint.x += ropeSegLen * ropeBuildDir.x * initialStretch;
        }
    }

    public Transform GetSegementObject(int no) {
        return segmentSprites[no];
    }

    void LateUpdate() {
        if (!active && !firstAcive) return;
        
        if (darwWithLineRenderer) DrawRopeLine();
        if (drawSegmentSprites) DrawSegmentSprites();
        Simulate();
        PostLateUpdate();

        // lets it run simulation once
        firstAcive = false;
    }

    public virtual void PostLateUpdate()  {}

    private void Simulate() {
        if (!active && !firstAcive) return;

        PreSimulate();
        // SIMULATION
        for (int i = 0; i < numSegments; i++) {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector2 velocity = (firstSegment.posNow - firstSegment.posOld);
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity * TimeScaled();
            if (i < 1) firstSegment.posNow += velocity;
            firstSegment.posNow += gravity * GTime.deltaTime;
            ropeSegments[i] = firstSegment;
        }

        //CONSTRAINS
        for (int i = 0; i < simulatonAccuracy; i++) {
            ApplyConstraints();
        }

        PostSimulate();
    }

    public virtual void PreSimulate() {}
    public virtual void PostSimulate() {}

    public virtual void AttachToObjects() {
        RopeSegment firstSegment = ropeSegments[0];

        // first position of rope is set to mouse pos
        if (attachEndOneToMouse) {
            firstSegment.posNow = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.ropeSegments[0] = firstSegment;
        }

        // attached to objects
        if (endOneAttach != null) {
            firstSegment.posNow = endOneAttach.position;
            this.ropeSegments[0] = firstSegment;
        }
        if (endTwoAttach != null) {
            RopeSegment lastSegment = ropeSegments[numSegments-1];
            lastSegment.posNow = endTwoAttach.position;
            this.ropeSegments[numSegments-1] = lastSegment;
        }
    }


    private void ApplyConstraints () {
        // attatch ends to objects
        AttachToObjects();
        

        // keep segments a constistant distance from eachother
        for (int i = 0; i < this.numSegments - 1; i++) {
            RopeSegment firstSeg = ropeSegments[i];
            RopeSegment secondSeg = ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - ropeSegLen);
            Vector2 changeDir = Vector2.zero;

            if (dist > ropeSegLen) {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen) {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector2 changeAmount = changeDir * error;
            if (i != 0) {
                firstSeg.posNow -= changeAmount * 0.5f; //* volitility;
                ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f; // * volitility;
                ropeSegments[i + 1] = secondSeg;
            }
            else {
                secondSeg.posNow += changeAmount;
                ropeSegments[i + 1] = secondSeg;
            }
            AddedConstraints();
        }
    }

    public virtual void AddedConstraints() {}

    public float TimeScaled() {
        return GTime.deltaTime/GTime.unscaledDeltaTime;
    }

    void DrawSegmentSprites() {
        float spriteAngle;
        for (int i = 0; i < numSegments; i++) {
            
            segmentSprites[i].position = ropeSegments[i].posNow;

            if (rotateSegSprites) {
                Vector2 angleDiff;

                if (i == 0) angleDiff = ropeSegments[i+1].posNow - ropeSegments[i].posNow;
                else angleDiff = ropeSegments[i].posNow - ropeSegments[i-1].posNow;

                spriteAngle = Vector2.Angle(Vector2.up, angleDiff);
                if (angleDiff.x > 0) spriteAngle *= -1;

                segmentSprites[i].eulerAngles = new Vector3(0,0,spriteAngle + spriteRotOffset);
            }
        }
    }

    void MakeSegmentSprites() {
        for (int i = 0; i < numSegments; i++) {
            SpriteRenderer segSr = new GameObject().AddComponent<SpriteRenderer>();
            segSr.sprite = segmentSprite;
            // end of whip
            if (i == numSegments-1) {
                if (endSprite) {
                    segSr.sprite = endSprite;
                }
                if (endOject) {
                    Transform t = Instantiate(endOject, segSr.transform.position, Quaternion.identity).transform;
                    t.parent = segSr.transform;
                }
            } 
            segSr.sortingLayerName = sortLayer;
            segSr.sortingOrder = startingSort + i;
            segmentSprites.Add (segSr.transform);
            segSr.transform.parent = transform;
        }
    }

    private void DrawRopeLine() {
        float lineWidth = 0.1f;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[numSegments];
        for (int i = 0; i < numSegments; i++) {
            ropePositions[i] = ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    public struct RopeSegment {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment (Vector2 pos) {
            this.posNow = pos;
            this.posOld = pos;
        }
    }

    public Vector2 GetEndPoint() {
        return ropeSegments[numSegments-1].posNow;
    }
}
