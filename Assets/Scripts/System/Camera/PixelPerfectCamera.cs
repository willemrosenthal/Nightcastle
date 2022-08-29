using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPerfectCamera : MonoBehaviour {

	public bool pixelPerfectSize = true;
	public float gamePixelHeight = 224;
	public float gamePixelWidth = 384;

	public bool pixelPerfectPosition = true;
	public float pixPerUnit = 32f;
	public bool debug = false;
	Vector2 pixelPerfectPos;
	Vector2 realPos;

	Bounds bounds2d;

	GameManager gm;
	Camera cam;

	void Awake () {
		gm = GameManager.Instance;
		cam = GetComponent<Camera> ();
		//gamePixelWidth = gm.screenSize.x;
		//gamePixelHeight = gm.screenSize.y;
	}

	void Start() {
		if (pixelPerfectSize)
			cam.orthographicSize = ((float)gamePixelHeight / pixPerUnit) * 0.5f;

		bounds2d = new Bounds(
			transform.position,
			new Vector3(gamePixelWidth * (1f/(float)pixPerUnit), gamePixelHeight * (1f/(float)pixPerUnit), 0));
	}

	void OnPreRender() {
		realPos = (Vector2)transform.position;

		if (!pixelPerfectPosition)
			return;
		pixelPerfectPos = realPos;
		pixelPerfectPos.x = Mathf.Floor (pixelPerfectPos.x * pixPerUnit) / pixPerUnit;
		pixelPerfectPos.y = Mathf.Floor (pixelPerfectPos.y * pixPerUnit) / pixPerUnit;

		transform.position = (Vector3)pixelPerfectPos + Vector3.forward * transform.position.z;

	}

	void OnPostRender() {
		bounds2d.center = (Vector2)transform.position;
		transform.position = (Vector3)realPos + Vector3.forward * transform.position.z;
	}

	public bool SpriteOnCamera(Bounds spriteBounds) {
		spriteBounds.center = (Vector2)spriteBounds.center;
		if (bounds2d.Intersects (spriteBounds))
			return true;
		return false;
	}

	void OnDrawGizmos() {
		if (debug) {
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube (bounds2d.center, bounds2d.size);
		}
	}
}
