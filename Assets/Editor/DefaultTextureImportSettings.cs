using UnityEngine;
using UnityEditor;

public class SpritePixelsPerUnitChanger : AssetPostprocessor
{
	void OnPreprocessTexture ()
	{
		TextureImporter textureImporter  = (TextureImporter) assetImporter;
		textureImporter.spritePixelsPerUnit = 32;
		textureImporter.filterMode = FilterMode.Point;
		textureImporter.mipmapEnabled = false;
		textureImporter.maxTextureSize = 2048;
		textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
		//textureImporter.textureFormat = TextureImporterFormat.ARGB32;
	}
}