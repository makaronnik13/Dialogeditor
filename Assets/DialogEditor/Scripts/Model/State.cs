using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[System.Serializable]
public class State: ScriptableObject
{
    [SerializeField]
	private string _description;
	public string description{
		get
		{
			return _description;
		}
		set
		{
			if(value!="")
			{
                string ss = value.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
                ss = ss.Substring(0, Mathf.Min(10, ss.Length));
                if (name!=ss) {
                    name = ss;
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            _description = value;
        }
	}
	public List<Path> pathes = new List<Path>();
	public Rect position;
	public AudioClip sound;

	public void Init(Chain chain)
	{
        Debug.Log(GuidManager.getGameByChain(chain));
		description = "";
        float z = GuidManager.getGameByChain(chain).zoom;
        position = new Rect(300,300,208*z,30*z);
	}

	public Path AddPath()
	{
		Path newPath = CreateInstance<Path> ();
		pathes.Add(newPath);
		AssetDatabase.AddObjectToAsset (newPath, AssetDatabase.GetAssetPath(this));
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
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

