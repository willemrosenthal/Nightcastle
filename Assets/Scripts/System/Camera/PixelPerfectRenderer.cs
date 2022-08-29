using UnityEngine;
using System.Collections;

public class PixelPerfectRenderer : MonoBehaviour {

	public FilterMode filterMode;

	 // 256 x 224 is SNES
	 // 342 or (341.333) = Widescreen version of snes resolution, accounting for stretching
	 // 398 or (398.222) = Widescreen version snes without stretching, I.E. square pixels
	 // SOTN = 256 x 208
	 // 320 x 208 = Close to SOTN res
	public Vector2 screenSize = new Vector2(352, 224); // was 342, but 352 fits full tiles on screen
	public Depth depth = Depth.twentyfour;
	Vector2 _currentScreenSize;

	// makes a backup to be loaded later
	Material screenMaterialBackup;

	public bool blitTwice = false;
	public bool showSquarePixels = false;

	// tells system to reclculate screen
	bool recalculateScreen = false;
	Vector2 unstretchedScreenSize;
	bool inSquarePxMode = false;
	//bool letterboxMade = false;

	// screen scaling, stretching and position
	public bool manuallyControlScreenOffset = false;
	public bool debugSwitchMode = false;
	public Vector2 screenScale = new Vector2(1,1);
	public Vector2 screenOffset  = new Vector2(0,0);
	public AnimationCurve offsetCurve;

	// materials and transitons
	public Material screenMaterial;
	//public Shader pixelSmoothing;
	//public int pxSmothingScale = 4;
	//[HideInInspector] public Material smoothMat;


	// PRIVATE VARS ----
	GameManager gm;
	private Camera renderCamera;
	private RenderTexture src;

	// DEST TEXUTRE
	private RenderTexture dest;

	private bool isFullScreen;

	// UNITY API ----
	void Awake () {
		renderCamera = GetComponent<Camera>();
		gm = GameManager.Instance;

		// switch / consoles must be full width
		#if UNITY_SWITCH && !UNITY_EDITOR
			screenSize.x = 342;
			screenSize.y = 226;
		#endif

		if (!enabled)
			return;

		RecacluateAspect();

		// get non-streched dimentions
		unstretchedScreenSize = new Vector2((float)screenSize.y * (1920f/1080f), screenSize.y);
		Debug.Log(unstretchedScreenSize);
		if (showSquarePixels) {
			//MakeBlackBarsForUnstretchedScreen();
			RecacluateAspect();
		}

		isFullScreen = Screen.fullScreen;
		SetRenderCam();
		
		screenMaterialBackup = screenMaterial;
	}

	public void RecacluateAspect() {
		renderCamera.orthographicSize = (screenSize.y / 64f);
		if (showSquarePixels) {
			//if (!letterboxMade) MakeBlackBarsForUnstretchedScreen();
			renderCamera.aspect = (unstretchedScreenSize.x / unstretchedScreenSize.y);
			inSquarePxMode = true;
			_currentScreenSize = unstretchedScreenSize;
		}
		else {
			renderCamera.aspect = (screenSize.x / screenSize.y);
			inSquarePxMode = false;
			_currentScreenSize = screenSize;
		}
		recalculateScreen = true;
	}


	void Update() {
		if ((!inSquarePxMode && showSquarePixels) || (inSquarePxMode && !showSquarePixels) ) RecacluateAspect();

		if (recalculateScreen) {
			SetRenderCam();
		}

		// have we toggled the windows/fullscreen mode?
		if(Screen.fullScreen != isFullScreen) {
			isFullScreen = !isFullScreen;
			SetRenderCam();
		}
	}

	void OnPreRender() {
		if (!enabled)
			return;
		// this ensures that w/e the camera sees is rendered to our RenderTexture
		renderCamera.targetTexture = src;

	}

	void OnPostRender() {
		if (!enabled)
			return;

		// for some reason we need to se the traget texture to null before we blit
		renderCamera.targetTexture = null;

		// calculations for if the video should appear smaller on screen, and where it is centered on screen. I.E. Shrinking video to not fill the full display area.
		// this is usefulm for TV settings where some of the screen may be cut off.
		if (screenScale.x < 0.5) screenScale.x = 0.5f;
		if (screenScale.y < 0.5) screenScale.y = 0.5f;
		if (screenScale.x > 1f) screenScale.x = 1f;
		if (screenScale.y > 1f) screenScale.y = 1f;
		Vector2 scaledScreen = new Vector2(1/screenScale.x, 1/screenScale.y);
		float xOffset = offsetCurve.Evaluate( Math.PercentBetween(1,0.5f, screenScale.x) );
		float yOffset = offsetCurve.Evaluate( Math.PercentBetween(1,0.5f, screenScale.y) );
		//float xOffset = src.si
		if (!manuallyControlScreenOffset) screenOffset = new Vector2(-xOffset, - yOffset);

		if (blitTwice) {
			dest =  RenderTexture.GetTemporary((int)_currentScreenSize.x, (int)_currentScreenSize.y, DepthCalc(depth));
			dest.filterMode = filterMode;

			// casting null as a RenderTexture allows us to blit to the screen
			Graphics.Blit(src, dest, screenMaterial);

			Graphics.Blit(dest, null as RenderTexture, scaledScreen, screenOffset);
			RenderTexture.ReleaseTemporary(dest);
		}
		else {
			dest =  RenderTexture.GetTemporary((int)_currentScreenSize.x, (int)_currentScreenSize.y, DepthCalc(depth));
			dest.filterMode = filterMode;

			Graphics.Blit(src, dest, screenMaterial);
			
			Graphics.Blit(dest, null as RenderTexture, scaledScreen, screenOffset);
			RenderTexture.ReleaseTemporary(dest);
		}
	}

	public int DepthCalc(Depth d) {
		if(d == Depth.none) {
			return 0;
		}
		else if(d == Depth.sixteen) {
			return 16;
		}
		else if(d == Depth.twentyfour) {
			return 24;
		}
		return 0;
	}


	public void SetEffectMaterial(Material _mat, bool _blitTwice = true) {
		screenMaterial = _mat;
		blitTwice = _blitTwice;
	}

	public void ResetEffectMaterial() {
		screenMaterial = screenMaterialBackup;
		blitTwice = false;
	}

	public void SetRenderCam() {
		recalculateScreen = false;

		if (!renderCamera) {
			renderCamera = GetComponent<Camera>();
		}

		Vector2 _screenSize = screenSize;

		if (showSquarePixels) {
			_screenSize = unstretchedScreenSize;
		}

		src = new RenderTexture((int)_screenSize.x, (int)_screenSize.y, DepthCalc(depth));
		src.filterMode = filterMode;
		renderCamera.targetTexture = src;
	}

	public Vector2 GetCameraSize () {
		float width = (float)_currentScreenSize.x * 1f/32f;
		float height = Camera.main.orthographicSize * 2;
		return new Vector2(width, height);
	}


	// CURRENTLY UNSED
	void MakeBlackBarsForUnstretchedScreen() {
		GameObject blackBar = new GameObject();
		SpriteRenderer sr = blackBar.AddComponent<SpriteRenderer>();
		sr.sortingLayerName = "Cursor";
		sr.sortingOrder = 1000000;
		sr.sprite = (Sprite)Resources.Load("UI/unstretched-screen-black-bar", typeof(Sprite));
		blackBar.transform.parent = transform;
		blackBar.transform.localPosition = Vector2.right * screenSize.x * 0.5f * gm.settings.pixelUnit;
		blackBar.transform.localPosition += Vector3.down * screenSize.y * 0.5f * gm.settings.pixelUnit;
		blackBar.transform.localPosition += Vector3.forward;
		blackBar.transform.localScale = Vector2.one * 6;

		blackBar = new GameObject();
		sr = blackBar.AddComponent<SpriteRenderer>();
		sr.sortingLayerName = "Cursor";
		sr.sortingOrder = 1000000;
		sr.sprite = (Sprite)Resources.Load("UI/unstretched-screen-black-bar", typeof(Sprite));
		sr.flipX = true;
		blackBar.transform.parent = transform;
		blackBar.transform.localPosition = Vector2.left * screenSize.x * 0.5f * gm.settings.pixelUnit;
		blackBar.transform.localPosition += Vector3.down * screenSize.y * 0.5f * gm.settings.pixelUnit;
		blackBar.transform.localPosition += Vector3.forward;
		blackBar.transform.localScale = Vector2.one * 6;

		//letterboxMade = true;
	}

	// stuct
	public enum Depth {
		none,
		sixteen,
		twentyfour
	};

}
