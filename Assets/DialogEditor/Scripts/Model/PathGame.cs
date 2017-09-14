using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PathGame", menuName = "PathGame")]
[System.Serializable]
public class PathGame: ScriptableObject
{
    public string name;
    public string description;
    public string autor;
	[HideInInspector]
    public List<Chain> chains = new List<Chain>();
	[HideInInspector]
    public List<Param> parameters = new List<Param>();
    [HideInInspector]
    public float zoom = 1;
}