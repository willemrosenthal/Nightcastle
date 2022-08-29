using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
	
	public bool shaking;
	public Vector2 frameShake;

	//[Foldout("Self Positioning for Non-Camera use", true)] 
	public bool selfPositioning = false;
	public bool relativeSelfPositioning;
	float shakeTimer;
	float shakeAmount;
	float shakeDecreasePercent;
	bool shakeUnscaledTime;
	float xShakeMod;
	float yShakeMod;
	Vector3 shake;

	float sinTime;
	float sinTotalTime;
	bool sinUnscaledTime;
	Vector2 amp;
	Vector2 maxAmp;
	Vector2 freq;
	Vector2 defaultFreq = new Vector2(60,60);
	bool sinShakeDecay;

	float pxUnit = (1f/32f);

	GameManager gm;

	void Awake() {
		gm = GameManager.Instance;
	}

	void Update() {
		if (selfPositioning) {
			if (!relativeSelfPositioning)
				UpdateCameraShake(transform.position, false);
			if (relativeSelfPositioning)
				UpdateCameraShake(transform.localPosition, true);
		} 
	}

	public void UpdateCameraShake(Vector3 realPos, bool relative = false) {
		if (!relative) transform.position = realPos;
		else transform.localPosition = realPos;
		Shake();
		SinShake();
	}

	public void SinShakeCamera (Vector2 _shakeAmmount, float _shakeTime, float freqMod = 1, bool _shakeDecay = true, bool _unscaledTime = true, bool forceReset = false) {
		if (shaking && !forceReset) return;
		sinTime = 0;
		sinTotalTime = _shakeTime;
		freq = defaultFreq * freqMod;
		sinShakeDecay = _shakeDecay;
		sinUnscaledTime = _unscaledTime;
		amp = -_shakeAmmount * pxUnit * gm.settings.screenShake;
		maxAmp = amp;
		shaking = true;
	}
	
	float SinWave(float amplitude, float frequency) {
		return amplitude * Mathf.Sin (frequency * sinTime);
	}

	void SinShake() {
		if (sinTotalTime > 0 || sinTotalTime == -1) {

			Vector2 sinShakeVal = new Vector2(SinWave(amp.x, freq.x), SinWave(amp.y, freq.y));
			transform.position += (Vector3)sinShakeVal;
			frameShake = sinShakeVal;

			if (sinShakeDecay)
				amp = Vector2.Lerp(maxAmp, Vector2.zero, sinTime/sinTotalTime);

			if (sinUnscaledTime) {
				sinTime += GTime.unscaledDeltaTime;
			}
			else {
				sinTime += GTime.deltaTime;
			}

			if (sinTime > sinTotalTime && sinTotalTime != -1) {
				sinTotalTime = 0;
				shaking = false;
			}
		}
	}

	public void UpdateSinShakeVals(Vector2 _shakeAmmount, float _addTime) {
		amp = -_shakeAmmount * pxUnit * gm.settings.screenShake;
		maxAmp = amp;
		sinTotalTime += _addTime;
	}
	
	public void UpdateShakeVals(float _shakeAmmount, float _addTime) {
		shakeAmount = _shakeAmmount;
		shakeTimer += _addTime;
	}

	public void ShakeCamera (float _shakeAmmount, float _shakeTime, float _decreaseShakeByPercent = 1, bool _unscaledTime = false, float _xShakeMod = 1, float _yShakeMod = 1) {
		shakeTimer = _shakeTime;
		shakeAmount = _shakeAmmount;
		shakeDecreasePercent = _decreaseShakeByPercent;
		shakeUnscaledTime = _unscaledTime;
		xShakeMod = _xShakeMod;
		yShakeMod = _yShakeMod;
		shaking = true;
	}

	void Shake() {
		if (shakeTimer > 0) {
			shake = new Vector3 (Math.PlusOrMinus () * shakeAmount * Random.value * gm.settings.screenShake * xShakeMod, Math.PlusOrMinus () * shakeAmount * Random.value * gm.settings.screenShake * yShakeMod);
			if (!shakeUnscaledTime) {
				shakeTimer -= GTime.deltaTime;
				shake *= GTime.deltaTime;
			}
			if (shakeUnscaledTime) {
				shakeTimer -= GTime.unscaledDeltaTime;
				shake *= GTime.unscaledDeltaTime;
			}
			transform.position += shake;
			frameShake = shake;
			shakeAmount *= shakeDecreasePercent;

			if (shakeTimer <= 0)
				shaking = false;
		} 
	}


	public void StopShaking() {
		shakeTimer = 0;
		sinTotalTime = 0;
		shaking = false;
	}

}
