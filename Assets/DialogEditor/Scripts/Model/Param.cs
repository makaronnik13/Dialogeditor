using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[System.Serializable]
public class Param: ScriptableObject
{
    [SerializeField]
    private string _paramName = "new param";
	public string paramName
	{
		get{
			return _paramName;
		}
		set{
			_paramName = value;
            if (name != _paramName)
            {
                name = _paramName;
                GuidManager.getGameByParam(this).Dirty = true;
            }
        }
	}
	public bool showing;
	public string description = "";
	public Sprite image;
    public bool withChange;
	public int id;

    public List<ConditionChange> autoActivatedChangesGUIDS = new List<ConditionChange>();

    //use guid
    public Vector2 scrollPosition;
	public int paramGUID;
}