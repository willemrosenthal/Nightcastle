using System.Collections;
using UnityEngine;

public class DestroyFinishedParticle : MonoBehaviour {

	public bool realWorldSpace = false;
	private ParticleSystem particle;
	private ParticleSystemRenderer pRenderer;
	public string layer = "";
	public bool dontUseRandomSeed = true;
	public bool autoSize = true;
	public bool autoPlay = true;
	public bool maintainScaleToRateRatio = false;

	public AudioClip loopSound;
	float loopTimer;

	float scaleToRateRatio;
	float pixelsToMeter = 32;

	public float destroyAfterTime = 0;
	float destroyTime;

	public bool dontDestroyOnStop = false;

	ParticleSystem.TextureSheetAnimationModule tsam;

	ParticleSystem.EmissionModule emission;
	Renderer renderer;
	GameManager gm;
	//AudioManager am;

	void Awake () {
		particle = GetComponent<ParticleSystem> ();
		pRenderer = GetComponent<ParticleSystemRenderer> ();
		renderer = GetComponent<Renderer> ();
		gm = GameManager.Instance;
		//am = gm.am;

		if (layer != "") {
			renderer.sortingLayerName = layer;
		}
		if (dontUseRandomSeed) {
			//particle.Stop();
			particle.Stop (true, ParticleSystemStopBehavior.StopEmittingAndClear);
			particle.randomSeed = (uint)Random.Range(0,9999999);
		}
		if (autoSize && pRenderer.material.mainTexture) {
			Resize ();
		}
		if (autoPlay)
			particle.Play();

		if (maintainScaleToRateRatio) {
			emission = particle.emission;
			scaleToRateRatio = emission.rateOverTime.constant; 
			emission.rateOverTime = scaleToRateRatio * particle.shape.scale.x * particle.shape.scale.y;		
		}
	}

	public void Resize() {
		float textureVertSize = pRenderer.material.mainTexture.height / particle.textureSheetAnimation.numTilesY;
		particle.startSize = textureVertSize / pixelsToMeter;
	}

	void Start() {
		destroyTime = destroyAfterTime;
	}

	void Update () {
		if (loopSound) {
			loopTimer-= Time.deltaTime;
			if ( loopTimer <= 0) {
				loopTimer = particle.main.duration;
				//am.Play(loopSound);
			}
		}

		if (dontDestroyOnStop) return;

		if (destroyAfterTime > 0) {
			destroyTime -= Time.deltaTime;
			if (destroyTime <= 0)
				Destroy (gameObject);
		}
			
		if (!particle.isPlaying) {
			Destroy (gameObject);
		};
	}

	void LateUpdate () {
		if (realWorldSpace) {
			renderer.sortingOrder = (int)Camera.main.WorldToScreenPoint (transform.position).y * -1;
		}
	}

	public void SetLayer(string layerName) {
		layer = layerName;
		renderer.sortingLayerName = layerName;
	}
}
