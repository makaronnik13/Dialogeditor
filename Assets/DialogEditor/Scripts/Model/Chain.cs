using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Chain: ScriptableObject
{
	public string dialogName
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	[HideInInspector]
	public State StartState;
	[HideInInspector]
    public State inspectedState;
	[HideInInspector]
	public List<State> states = new List<State>();
	[HideInInspector]
	public List<StateLink> links = new List<StateLink> ();

	public StateLink AddStateLink()
	{
		StateLink sl = CreateInstance<StateLink> ();
		sl.Init (this);
		AssetDatabase.AddObjectToAsset (sl, AssetDatabase.GetAssetPath(this));
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		links.Add (sl);
		return sl;
	}

	public void RemoveStateLink(StateLink link)
	{
		links.Remove (link);
		Undo.DestroyObjectImmediate (link);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
	}

	public State AddState()
	{
		State newState = CreateInstance<State> ();
		AssetDatabase.AddObjectToAsset (newState, AssetDatabase.GetAssetPath(this));
        newState.Init(this);
        AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		states.Add (newState);
		return newState;
	}

	public void RemoveState(State state)
	{
		if (state == StartState)
		{
			if (states.Count == 1)
			{
				StartState = AddState();
			}
			else
			{
				foreach (State s in states)
				{
					if (s!=state)
					{
						StartState = s;
						break;
					}
				}
			}
		}

		states.Remove (state);
		state.DestroyState ();
		Undo.DestroyObjectImmediate(state);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
	}

	public void DestroyChain()
	{
		foreach(State s in states)
		{
			s.DestroyState ();
			Undo.DestroyObjectImmediate(s);
		}
	}

	public void Init(PathGame game)
	{
		Debug.Log ("int");
		dialogName = "New chain";
		name = dialogName;
		AssetDatabase.AddObjectToAsset (this, AssetDatabase.GetAssetPath(game));
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
        game.chains.Insert(0, this);
        StartState = AddState();
	}
}
