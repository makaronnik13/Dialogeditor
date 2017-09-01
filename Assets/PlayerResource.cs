using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerResource : Singleton<PlayerResource> {

	private Dictionary<Param, float> paramsDictionary = new Dictionary<Param, float>();

	public void Load()
	{
		
	}

	public void Save()
	{
		
	}

	public void Start()
	{
		DialogPlayer.Instance.onPathGo += new DialogPlayer.PathEventHandler(ChangeParams);
	}

	public void ChangeParams(Path path)
	{
		foreach(ParamChanges pch in path.changes)
		{
			List<float> playerParameters = new List<float> ();

			foreach(Param p in pch.Parameters)
			{
				if(!paramsDictionary.ContainsKey(pch.aimParam))
				{
					paramsDictionary.Add (pch.aimParam, 0);
				}	
			}

			if (!paramsDictionary.ContainsKey (pch.aimParam)) {

				foreach(Param p in pch.Parameters)
				{
					//playerParameters.Add ();
					playerParameters.Add (paramsDictionary[p]);
				}


				paramsDictionary.Add (pch.aimParam, ExpressionSolver.CalculateFloat ());
			} else 
			{
				paramsDictionary[pch.aimParam] = ExpressionSolver.CalculateFloat ();
			}
		}
	}

	public bool CheckCondition(Condition condition)
	{
		List<float> playerParameters = new List<float> ();

		foreach(Param p in condition.Parameters)
		{
			if(!paramsDictionary.ContainsKey(p))
			{
				paramsDictionary.Add (p, 0);
			}	
			playerParameters.Add (paramsDictionary[p]);
		}

		return ExpressionSolver.CalculateBool (condition.conditionString, playerParameters);
	}
}
