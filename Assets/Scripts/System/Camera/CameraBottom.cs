using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBottom : MonoBehaviour
{
    public Vector2 offsetBeginPoint;

    public bool useTop = false;

    Vector3 finalOffsetPos;
    Vector2 worldBeginPt;
    float leftX;
    float rightX;

    CameraBounds cameraBounds;

    void Start() {
        cameraBounds = GameManager.Instance.cameraBounds;

        worldBeginPt = (Vector2)transform.position + offsetBeginPoint;
        leftX = offsetBeginPoint.x < 0 ? worldBeginPt.x : transform.position.x;
        rightX = offsetBeginPoint.x < 0 ? transform.position.x : worldBeginPt.x;

        float offsetDir = !useTop ? 1 : -1;
        finalOffsetPos = new Vector3(transform.position.x, transform.position.y + cameraBounds.bounds2D.size.y * 0.5f * offsetDir, Camera.main.transform.position.z);
    }

    void LateUpdate() {
        // check if right or left:
            // if offsetBeginPoint.x < 0 then use right side of cam else use left side
            // CamRegisterPt = the lower right or lower left point on the camera.
        Vector2 camRegisterPt = (offsetBeginPoint.x < 0)? new Vector2(cameraBounds.bounds2D.max.x, cameraBounds.bounds2D.min.y) : (Vector2)cameraBounds.bounds2D.min;
        if (useTop) camRegisterPt.y = cameraBounds.bounds2D.max.y;

        // if CamRegisterPt is between the two points
        bool insideLeftAndRight = cameraBounds.bounds2D.max.x >= leftX && cameraBounds.bounds2D.min.x <= rightX;
        bool insideYrange = !useTop? Camera.main.transform.position.y < finalOffsetPos.y : Camera.main.transform.position.y > finalOffsetPos.y;
        if (insideLeftAndRight && insideYrange && cameraBounds.bounds2D.min.y <= transform.position.y && cameraBounds.bounds2D.max.y >= transform.position.y) {
            float percentBetween = Math.PercentBetween(leftX, rightX, camRegisterPt.x);
            finalOffsetPos.x = Camera.main.transform.position.x;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, finalOffsetPos, percentBetween);
        }
            // float percentBetween = then calculate the % bewteen the two and save
            // move the camera's y to that % between where it wants to be and where the current transform is (of this object)
        
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawWireSphere(transform.position, 1);
        Gizmos.DrawWireSphere(transform.position, 1.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (Vector3)offsetBeginPoint, 0.5f);
        Gizmos.DrawWireSphere(transform.position + (Vector3)offsetBeginPoint, 1);
        Gizmos.DrawWireSphere(transform.position + (Vector3)offsetBeginPoint, 1.5f);
    }
}
