using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueText : MonoBehaviour {

    // singleton
	public static DialogueText Instance { get; private set; }

    [TextArea]
    public string text = "";
    public string font = "Dialouge_English-A";

    float maxLinePxSize = 224;
    float maxLineSize;

    float textY = -2.85f;
    float lineHeight = (1f/32f) * 14;

    // writing
    string state = "";

    // interal. Only visible for debug
    [TextArea] public string textToWrite;
    [TextArea] public string currentLine;
    float scrollLineTimer;
    float scrollLineTime = 0.2f;

    // interrupt
    bool interrupted;
    float interruptTimer = 0;
    float interruptTime = 0.5f;

    GameObject lineObj;
    DialogueLine testLine;

    int maxLines = 2;
    List<DialogueLine> lines = new List<DialogueLine>();


    PlayerInputs playerInputs;

    void Awake() {
        Singleton();
        state = "";
    }

    void Singleton () {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy (gameObject);
        }
    }

    void Start() {
        playerInputs = GameManager.Instance.playerInputs;

        // get line sise in units
        maxLineSize = maxLinePxSize * (1f/32f);

        // go inside camera and position accordingly
        transform.parent = Camera.main.transform;
        transform.localPosition = new Vector3(0,0,10);
    }

    public void Initilize(string textToSay) {
        // build line tester
        LineTeter();

        Debug.Log(textToSay);

        // set up text tos ay
        text = textToSay;

        // all text needs to be written
        textToWrite = text;

        // get ready
        state = "ready";
    }

    void LineTeter() {
        string pathToFont = "Fonts/" + font;
		lineObj = (GameObject)Resources.Load(pathToFont, typeof(GameObject));
        testLine = Instantiate(lineObj, transform.position, Quaternion.identity).GetComponent<DialogueLine>();
        testLine.Hide();
    }

    void Update() {
        if (state == "ready") {
            GetLine();
            CreateLine();
            state = "writing";
        }
        else if (state == "writing") {
            if (lines[0].complete) {
                if (playerInputs.D.WasPressed) {
                    state = "next";
                }
            }  
        }
        else if (state == "next") {
            if (textToWrite.Length == 0) {
                state = "done";
            }
            else {
                state = "scroll";
                scrollLineTimer = 0;
            }
        }
        else if (state == "scroll") {
            scrollLineTimer += GTime.deltaTime;
            for (int i = 0; i < lines.Count; i++) {
                float newY = Mathf.Lerp((float)i * lineHeight + textY, (float)(i+1) * lineHeight + textY, scrollLineTimer/scrollLineTime);
                lines[i].transform.localPosition = new Vector3(lines[i].transform.localPosition.x, newY, 0);
            }
            if (scrollLineTimer > scrollLineTime) {
                // remove extra lines
                if (lines.Count >= maxLines) {
                    Destroy(lines[lines.Count-1].gameObject);
                    lines.RemoveAt(lines.Count-1);
                }
                // continue
                state = "ready";
            }
        }
        else if (state == "done") {
            EndDialogue();
        }
        else if (state == "interrupt") {
            interruptTimer -= Time.deltaTime;
            if (interruptTimer < 0) {
                EndDialogue();
            }
        }
        // step 1 are we ready to write?
        // step 2: get line, create dialouge line, shorten text to write
        // wait for line to complete. then either wait for time, or wait for button press. and begin agian.
        // if textToWrite is empty. wait for button press to end.

    }

    void GetLine() {
        int _lastSpace = 0;
        int _current = 0;
        float _lineWidth = 0;
        string _textToTest = "";

        while (_lineWidth < maxLineSize) {
            _textToTest += textToWrite[_current];
            _lineWidth = testLine.GetLineWidth(_textToTest);
            if (textToWrite[_current] + "" == " ") _lastSpace = _current;
            Debug.Log(textToWrite[_current].ToString() == "\n");
            _current++;

            // if we exceed the line size
            if (_lineWidth > maxLineSize) {
                currentLine = textToWrite.Substring(0, _lastSpace);
                textToWrite = textToWrite.Substring(_lastSpace+1);
                break;
            }
            // if line break
            else if (textToWrite[_current-1].ToString() == "\n") {
                currentLine = textToWrite.Substring(0, _current-1);
                if (textToWrite.Length > _current) textToWrite = textToWrite.Substring(_current);
                else textToWrite = "";
                break;
            }
            // if we finished
            else if (_textToTest.Length == textToWrite.Length) {
                currentLine = textToWrite;
                textToWrite = "";
                break;
            }

        }
    }

    void CreateLine() {
        DialogueLine newLine = Instantiate(lineObj, transform.position, Quaternion.identity).GetComponent<DialogueLine>();
        newLine.transform.parent = transform;
        newLine.transform.localPosition = new Vector3(-maxLineSize * 0.5f, textY, 0);
        newLine.line = currentLine;
        lines.Insert(0, newLine);
        newLine.StartWriting();
    }

    public void EndDialogue () {
        Destroy(this.gameObject);
    }

    public void Interrupt() {
        if (!interrupted) {
            interrupted = true;
            state = "interrupt";
            if (lines[0].Interrupt()) {
                interruptTimer = interruptTime;
            }
            else {
                interruptTimer = 0;
            }
        }
    }

    // load font object
    // create an invisble text object that's only for testing line size.
    // write out line (in a single frame so it's invisible) until either:
        // a line break is reached
        // the line is full, I.E. maximum size is reached. As line is written to check to see for max size, record the location of the last "space" charachter.
        // if line max is reached, rewind current palce to the "space" and create a line break there.

    // if array of lines contains any lines, scroll all lines up by 1 line height, over a .25 sec time.
    // create a new text line, which has code to write out it's single line, chachter by charachter and feed it the line we chose. Save this line in an array of lines.
    // if line list exceeds 2 in length, delete the oldest one (first place?)
    // now begin ACTUALLY writing the line, going charachter by charachter until line is compelte.
    // repeat
}
