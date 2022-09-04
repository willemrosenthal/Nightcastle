using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellListener : MonoBehaviour {
    
	public static SpellListener Instance { get; private set; }

    public bool listening;

    public string spell;

    // beat
    public float beatSpeedAdjust = 1;
    public float beat = 0.5f;
    public AudioClip beatSound;
    float beatTimer;

    PlayerInputs playerInputs;
    AudioManager am;

    void Awake() {
        Singleton();
        // if a duplicate, end code here
		if (Instance != this) return;

        SpellList.Initilize();
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
        am = GameManager.Instance.am;
        playerInputs = GameManager.Instance.playerInputs;
    }

    void Update() {
        if (!listening) {
            return;
        }

        Beat();
    }

    void Beat() {
        // beat occurs in REAL time.
        beatTimer -= Time.deltaTime;
        if (beatTimer < 0) {
            HeartBeat();
            beatTimer += beat * beatSpeedAdjust;
            AddToSpell();
            //Invoke("AddToSpell", 0.1f);
        } 
    }

    void AddToSpell () {
        Vector2 input = playerInputs.Dpad;
        string add = "•";
        if (input.y > 0 && input.x == 0) add = "^";
        else if (input.x > 0 && input.y == 0) add = ">";
        else if (input.y < 0 && input.x == 0) add = "v";
        else if (input.x < 0 && input.y == 0) add = "<";
        spell += add;
    }

    public void StartListening() {
        listening = true;
        beatTimer = beat * 0.5f * beatSpeedAdjust; // always start on half beat
        //if (beatSound) am.Play(beatSound);
    }

    public void EndListening() {
        listening = false;
        ExecuteSpell ();
    }

    void HeartBeat() {
        float _beatSpeedChange = Mathf.Clamp(beatSpeedAdjust, 0.4f, 1.4f);
        if (beatSound) am.Play(beatSound, 1, 0, 0, 1f/_beatSpeedChange);
    }

    void ExecuteSpell () {
        // executes spell
        Debug.Log("TRY SPELL: " + spell + " RESULT: " + SpellList.GetSpell(spell));
        // resets spell
        spell = "";
    }

    void OnGUI () {
        if (listening) {
            GUIStyle style = new GUIStyle();
            style.fontSize = 72;
            style.normal.textColor = Color.white;
            style.normal.background = MakeTex( 2, 2, new Color( 0f, 0f, 0f, 0.5f ) );
            style.padding = new RectOffset(25,25,0,0);

            spell = GUI.TextArea (new Rect (Screen.width * 0.2f, Screen.height - 80, Screen.width * 0.6f, 80), spell, 100, style);
        }
    }

    private Texture2D MakeTex( int width, int height, Color col ) {
        Color[] pix = new Color[width * height];
        for( int i = 0; i < pix.Length; ++i )
        {
            pix[ i ] = col;
        }
        Texture2D result = new Texture2D( width, height );
        result.SetPixels( pix );
        result.Apply();
        return result;
    }
}
