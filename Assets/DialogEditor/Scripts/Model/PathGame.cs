using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PathGame", menuName = "PathGame")]
[System.Serializable]
public class PathGame: ScriptableObject
{
    public string gameName
    {
        get
        {
            return name;
        }
        set
        {
            if (name != value)
            {
                name = value;
                string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
                AssetDatabase.RenameAsset(assetPath, assetPath.Replace(assetPath, name));
            }
        }
    }
    public string description;
    public string autor;
	[HideInInspector]
    public List<Chain> chains = new List<Chain>();
	[HideInInspector]
    public List<Param> parameters = new List<Param>();
    [HideInInspector]
    public float zoom = 1;
}