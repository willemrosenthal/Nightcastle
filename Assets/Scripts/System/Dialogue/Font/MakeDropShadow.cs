using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeDropShadow : MonoBehaviour {

    public bool remakeShadow = false;
    public Color fontColor = Color.white;
    public Color shadowColor = Color.black;
    public Color otherColor = Color.yellow;
    public Color otherColor2 = Color.black;

    public bool surrounded = false;
    public bool upperOffset = false;
    public bool extraDrop = false;

    [HideInInspector]public bool father = true;

    string currentText;

    float waitTic = 1;

    Transform shadowHolder; 

    //GameObject container;
    List<GameObject> shadow = new List<GameObject>();

    Vector2[] drops = new Vector2[] {
        new Vector2(0, -1),
        new Vector2(1, -1),
        new Vector2(1, 0),
    };

    Vector2[] surround = new Vector2[] {
        new Vector2(1, -1),
        new Vector2(0, -1),
        new Vector2(1, 1),
        new Vector2(-1, -1),
        new Vector2(-1, 1),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(-1, 0),
    };

    Vector2[] shadowLoc;

    TextMesh text;

    void Start() {
        if (!father) enabled = false;
        else {
            shadowHolder = new GameObject().transform;
            shadowHolder.position = transform.position;
            shadowHolder.parent = transform.parent;
        }

        text = GetComponent<TextMesh>();

        // make contianer
        // container = new GameObject();
        // container.transform.parent = transform;  
        // container.transform.position = transform.position;
        // transform.parent = container.transform;

        if (surrounded) shadowLoc = surround;
        else shadowLoc = drops;

        // make text shadow
        //MakeShadow();
        remakeShadow = true;
    }

    public void MakeShadow() {
        father = false;
        
        // reset z position
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        // destroy existing shadow
        for (int i = shadow.Count-1; i >= 0; i--) {
            if (shadow[i]) Destroy(shadow[i].gameObject);
            shadow.RemoveAt(i);
        }
        
        //text.text = "test" + "\n" + "teset"; <--  IT WORKS!
        currentText = text.text;
        //container.name = "TEXT:" + currentText;

        text.color = fontColor;

        float px = 1f/32f;
        for (int i = 0; i < shadowLoc.Length; i++) {
            GameObject dup = Instantiate(this.gameObject, (Vector2)transform.position, Quaternion.identity);
            shadow.Add(dup);

            dup.transform.position = (Vector2)transform.position + shadowLoc[i] * px;

            Destroy(dup.GetComponent<MakeDropShadow>());
            Destroy(dup.GetComponent<DialogueLine>());
            dup.GetComponent<TextMesh>().color = shadowColor;
            dup.GetComponent<Renderer>().sortingOrder = -2;
            //dup.GetComponent<PixelPerfectObject>().enabled = false;
            //dup.GetComponent<RectTransform>(). = new Vector3(10,10,10);
            dup.transform.parent = shadowHolder;
            //dup.transform.localScale = new Vector3(10,10,10);
        }
        transform.position += Vector3.back * 0.1f;

        if (upperOffset) {
            GameObject dup = Instantiate(this.gameObject, (Vector2)transform.position, Quaternion.identity);
            shadow.Add(dup);
            dup.transform.position = (Vector2)transform.position + new Vector2(-1,0) * px;
            Destroy(dup.GetComponent<MakeDropShadow>());
            Destroy(dup.GetComponent<DialogueLine>());
            dup.GetComponent<TextMesh>().color = otherColor;
            dup.GetComponent<Renderer>().sortingOrder = -1;
            dup.transform.parent = shadowHolder;
        }

        if (extraDrop) {
            GameObject dup = Instantiate(this.gameObject, (Vector2)transform.position, Quaternion.identity);
            shadow.Add(dup);
            dup.transform.position = (Vector2)transform.position + new Vector2(1,0) * px;
            Destroy(dup.GetComponent<MakeDropShadow>());
            Destroy(dup.GetComponent<DialogueLine>());
            dup.GetComponent<TextMesh>().color = otherColor2;
            dup.GetComponent<Renderer>().sortingOrder = -1;
            dup.transform.parent = shadowHolder;
        }
    }

    void LateUpdate() {
        if (waitTic > 0) {
            waitTic--;
            return;
        }
        if (remakeShadow || text.text != currentText) {
            MakeShadow();
            remakeShadow = false;
            //enabled = false;
        }

        /// keep in correct posiiton
        shadowHolder.transform.position = transform.position;
    }

    void OnDestroy() {
        if (shadowHolder) Destroy(shadowHolder.gameObject);
    }
}
