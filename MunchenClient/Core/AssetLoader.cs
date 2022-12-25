using System.Collections.Generic;
using System.IO;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace MunchenClient.Core
{
    
	internal static class AssetLoader
	{
		private static readonly Dictionary<string, object> assetCache = new Dictionary<string, object>();

		private static AssetBundle cachedAssetBundle = null;

		internal static void LoadAssetBundle(string filePath)
		{
			cachedAssetBundle = AssetBundle.LoadFromMemory_Internal(File.ReadAllBytes(filePath), 0u);
			cachedAssetBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
		}

        internal static Sprite LoadSpriteFromDisk(this string path)
        {
            if (string.IsNullOrEmpty(path)) { return null; }
            byte[] data = File.ReadAllBytes(path);
            if (data == null || data.Length <= 0) { return null; }
            Texture2D tex = new Texture2D(512, 512);
            if (!Il2CppImageConversionManager.LoadImage(tex, data)) { return null; } //this doesnt exist in this project somehow??
            Sprite sprite = Sprite.CreateSprite(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f, 0, 0, new Vector4(), false);
            sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return sprite;
        }

		internal static Texture2D LoadTexture(string textureName)
		{
			if (cachedAssetBundle == null)
			{
				return null;
			}
			string text = "Assets/" + textureName + ".png";
			if (assetCache.ContainsKey(text))
			{
				return (Texture2D)assetCache[text];
			}
			Texture2D texture2D = cachedAssetBundle.LoadAsset_Internal(text, Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
			texture2D.hideFlags |= HideFlags.DontUnloadUnusedAsset;
			assetCache.Add(text, texture2D);
			return texture2D;
		}

		internal static Material LoadMaterial(string materialName)
		{
			if (cachedAssetBundle == null)
			{
				return null;
			}
			string text = "Assets/" + materialName + ".mat";
			if (assetCache.ContainsKey(text))
			{
				return (Material)assetCache[text];
			}
			Material material = cachedAssetBundle.LoadAsset_Internal(text, Il2CppType.Of<Material>()).Cast<Material>();
			material.hideFlags |= HideFlags.DontUnloadUnusedAsset;
			assetCache.Add(text, material);
			return material;
		}

		internal static AudioClip LoadAudio(string audioName)
		{
			if (cachedAssetBundle == null)
			{
				return null;
			}
			string text = "Assets/" + audioName + ".mp3";
			if (assetCache.ContainsKey(text))
			{
				return (AudioClip)assetCache[text];
			}
			AudioClip audioClip = cachedAssetBundle.LoadAsset_Internal(text, Il2CppType.Of<AudioClip>()).Cast<AudioClip>();
			audioClip.hideFlags |= HideFlags.DontUnloadUnusedAsset;
			assetCache.Add(text, audioClip);
			return audioClip;
		}

		internal static GameObject LoadGameObject(string prefabName)
		{
			if (cachedAssetBundle == null)
			{
				return null;
			}
			string text = "Assets/" + prefabName + ".prefab";
			if (assetCache.ContainsKey(text))
			{
				return (GameObject)assetCache[text];
			}
			GameObject gameObject = cachedAssetBundle.LoadAsset_Internal(text, Il2CppType.Of<GameObject>()).Cast<GameObject>();
			gameObject.hideFlags |= HideFlags.DontUnloadUnusedAsset;
			assetCache.Add(text, gameObject);
			return gameObject;
		}
	}
}
