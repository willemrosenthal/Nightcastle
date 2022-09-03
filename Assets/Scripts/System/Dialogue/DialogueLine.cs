using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueLine : MonoBehaviour {

    public string line;
    int currentChar = 0;


    public bool complete { get; private set; }
    public bool writing { get; private set; }
    float timer;
    float letterTime = 0.03f;

    public TextMesh text;
    public Renderer textRenderer;
    public MakeDropShadow dropShadow;

    void Start() {
        text.text = "";
        text = GetComponent<TextMesh>();
        textRenderer = GetComponent<Renderer>();
    }

    void Update() {
        if (writing) {
            Write();
        }
    }

    void Write() {
        timer -= GTime.unscaledDeltaTime;
        if (timer < 0) {
            text.text += line[currentChar];
            currentChar++;

            // done writing
            if (currentChar >= line.Length) {
                writing = false;
                complete = true;
                Debug.Log("complete");
            }
            timer = letterTime;
        }
    }

    public void StartWriting(bool useDropShadow = true) {
        Debug.Log("start writing called");
        Show();
        writing = true;
        if (useDropShadow) dropShadow.enabled = true;
    }

    public bool Interrupt() {
        if (writing) {
            line = line.Substring(0, currentChar);
            line += "-";
            return true;
        }
        return false;
    }

    public void Hide() {
        textRenderer.enabled = false;
    }
    public void Show() {
        textRenderer.enabled = true;
        textRenderer.sortingLayerName = "HUD";
    }

    public float GetLineWidth(string _text = "") {
        float width = textRenderer.bounds.size.x;
        if (_text != "") {
            string _saved = text.text;
            text.text = _text;
            width = textRenderer.bounds.size.x;
            text.text = _saved;
        }
        return width;
    }
}
