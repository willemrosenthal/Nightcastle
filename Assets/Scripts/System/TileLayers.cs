using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TileLayers : MonoBehaviour {

	public Color finalColor = Color.white;
	Tilemap tileMap;


	void Awake () {
		tileMap = GetComponent <Tilemap> ();
	}

	void Start () {
		tileMap.color = finalColor;
	}

}
