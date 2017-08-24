using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[System.Serializable]
public class State
{
	public string description;
	public Sprite image;
	public List<Path> pathes = new List<Path>();
    public int stateGUID;
	public Rect position;

	public State(int guid)
	{
		description = "";
		position = new Rect(0,0,150,100);
        stateGUID = guid;
	}
}

