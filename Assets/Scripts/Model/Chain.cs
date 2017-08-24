using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Chain
{
	public string name;
	private State startState;
	public int ChainGuid;
    public State inspectedState;

    public State StartState
    {
        get
        {
            return states[0];
        }
        set
        {
            states.Remove(value);
            states.Insert(0, value);
        }
    }
	public List<State> states = new List<State>();
    public List<StateLink> statesLinks = new List<StateLink>();

    public List<StateLink> StateLinks
    {
        get
        {
            if (statesLinks == null)
            {
                statesLinks = new List<StateLink>();
            }
            return statesLinks;
        }
    }

    public Chain(int cGuid, int sGuid)
	{
		ChainGuid = cGuid;
		name = "New chain";
		startState = new State (cGuid);
		states.Add (startState);
	}
}
