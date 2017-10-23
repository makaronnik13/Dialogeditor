using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Param : ScriptableObject
{
    private PathGame game;
    public PathGame Game
    {
        get
        {
            return game;
        }
        set
        {
            game = value;
        }
    }
    public string _paramName = "new param";
    public string paramName
    {
        get
        {
            return _paramName;
        }
        set
        {
            _paramName = value;
            if (name != _paramName)
            {
                name = _paramName;
                game.Dirty = true;
            }
        }
    }
    public bool showing;
    public string description = "";
    public Sprite image;
    public bool withChange;
    public List<ConditionChange> autoActivatedChangesGUIDS = new List<ConditionChange>();
    public Vector2 scrollPosition;
    public int paramGUID;
	public string tags;
	public string[] Tags
	{
		get
		{
			if (tags == "") {
				return new string[0];
			} else {
				return tags.Split (',');
			}
		}
	}
}