using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[System.Serializable]
public class Param
{
	public delegate void ParamActivation(State state);
    public event ParamActivation OnParamActivation;

   
    public string name;
	public bool showing;
	public string description;
	public Sprite image;
    public bool withChange;

    public List<ConditionChange> autoActivatedChangesGUIDS = new List<ConditionChange>();

    //use guid
    public Vector2 scrollPosition;
	public int paramGUID;
    public float pValue;
    public float PValue
    {
        get
        {
            return pValue;
        }
        set
        {
                pValue = value;
                CheckConditions();
        }
    }

    private void CheckConditions()
    {
		/*
		foreach (KeyValuePair<Condition, State> pair in autoActivatedChains)
        {
            if (pair.Key.ConditionValue)
            {
                OnParamActivation(pair.Value);
            }
        }
        */
    }

	public Param(int Guid)
	{
		this.paramGUID = Guid;
		name = "new param";
		showing = false;
		description = "";
	}
}