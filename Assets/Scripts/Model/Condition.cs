using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Condition
{
	public List<Param> Parameters
	{
		get
		{
			List<Param> par = new List<Param> ();
			foreach(int pg in ParametersGUID)
			{
				par.Add (GUIDManager.GetItemByGuid(pg));
			}
			return par;
		}
		set
		{
			ParametersGUID = new List<int> ();
			foreach(Param p in value)
			{
				ParametersGUID.Add (p.paramGUID);
			}
		}
	}
	public List<int> ParametersGUID = new List<int>();

    public string conditionString = "";
    public bool ConditionValue
    {
        get
        {
            return ExpressionSolver.CalculateBool(conditionString, Parameters); ;
        }
    }


	public void RemoveParam(Param p)
	{
		ParametersGUID.Remove (p.paramGUID);
	}
	public void AddParam(Param p)
	{
		ParametersGUID.Add(p.paramGUID);
	}

    public void setParam(int j, Param param)
    {
        ParametersGUID[j] = param.paramGUID;
    }
}



