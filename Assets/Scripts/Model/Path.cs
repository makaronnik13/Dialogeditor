using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Path
{
	public string text = "";
	public bool auto = false;
    public bool waitInput = false;
	public Condition condition = new Condition();
    public List<ParamChanges> changes = new List<ParamChanges>();
    public int aimStateGuid;
    public Sprite pathSprite;

    public State aimState
    {
        get
        {
            return GUIDManager.GetStateByGuid(aimStateGuid);
        }
        set
        {
            aimStateGuid = value.stateGUID;
        }
    }

    public Path()
    {
    }
}


