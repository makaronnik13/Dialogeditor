using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[System.Serializable]
public class Param: ScriptableObject
{
    public string paramName = "new param";
	public bool showing;
	public string description = "";
	public Sprite image;
    public bool withChange;

    public List<ConditionChange> autoActivatedChangesGUIDS = new List<ConditionChange>();

    //use guid
    public Vector2 scrollPosition;
	public int paramGUID;
}