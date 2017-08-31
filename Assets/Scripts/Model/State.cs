using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[System.Serializable]
public class State: ScriptableObject
{
	public string description;
	public List<Path> pathes = new List<Path>();
	public Rect position;
	public AudioClip sound;

	public void Init()
	{
		description = "";
		position = new Rect(300,300,208,30);
	}

	public Path AddPath()
	{
		Path newPath = CreateInstance<Path> ();
		pathes.Add(newPath);
		AssetDatabase.AddObjectToAsset (newPath, AssetDatabase.GetAssetPath(this));
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();

		position = new Rect (position.position, new Vector2(Mathf.Max(208, 15*(pathes.Count+1)) ,position.size.y));
		return newPath; 
	}

	public void RemovePath(Path path)
	{
		pathes.Remove (path);
		DestroyImmediate (path, true);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
	}

	public void DestroyState()
	{
		int pc = pathes.Count;
		for(int i = pc-1; i>=0; i--)
		{
			RemovePath (pathes[i]);
		}
	}
}

