using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[System.Serializable]
public class State
{
	public string description;
	public List<Path> pathes = new List<Path>();
    public int stateGUID;
	public Rect position;
	public AudioClip sound;

	public State(int guid)
	{
		description = "";
		position = new Rect(0,0,150,100);
        stateGUID = guid;
	}
}

