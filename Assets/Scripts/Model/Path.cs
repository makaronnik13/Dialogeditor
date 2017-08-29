using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class Path: ScriptableObject
{
	public string text = "";
	public bool auto = false;
    public bool waitInput = false;
	public bool withEvent = false;
	public Condition condition = new Condition();
    public List<ParamChanges> changes = new List<ParamChanges>();
    public int aimStateGuid;
	public UnityEvent pathEvent;
	public State aimState;
}


