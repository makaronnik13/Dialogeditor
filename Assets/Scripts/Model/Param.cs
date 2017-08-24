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
	public string tags;
	public string description;
	public Sprite image;
    public ParamChanges manualUsingChange;
    public bool withChange;
	public int usableChainGuid;
	public Chain usableChain
	{
		get
		{
			return GUIDManager.GetChainByGuid (usableChainGuid);
		}
		set
		{
			usableChainGuid = value.ChainGuid;
		}
	}
	public int usableStateGuid;
	public State usableState
	{
		get
		{
			return GUIDManager.GetStateByGuid(usableStateGuid);
		}
		set
		{
			usableStateGuid = value.stateGUID;
		}
	}

	public bool activating;
	public bool manualActivationWithState = false;
	public Condition manualUsingCondition = new Condition();


	public Dictionary<Condition, State> autoActivatedChains
	{
		get
		{
			Dictionary<Condition, State> ret = new Dictionary<Condition, State>();
			foreach(ConditionChain kc in autoActivatedChainsGUIDS)
			{
				ret.Add (kc.c, GUIDManager.GetStateByGuid(kc.stateGuid));
			}
			return ret;
		}
		set
		{
			autoActivatedChainsGUIDS = new List<ConditionChain>();
			foreach (KeyValuePair<Condition, State> kvp in value)
			{
				autoActivatedChainsGUIDS.Add (new ConditionChain(kvp.Key, GUIDManager.GetChainByStateGuid (kvp.Value.stateGUID).ChainGuid ,kvp.Value.stateGUID));
			}
		}
	}

    public void SetAutoActivatedChain(int id, Chain c)
    {
		autoActivatedChainsGUIDS[id].chainGuid = c.ChainGuid;
    }
	public void SetAutoActivatedState(int id, State s)
	{
		autoActivatedChainsGUIDS[id].stateGuid = s.stateGUID;
	}

	public void RemoveAutoActivatedChain(int id)
	{
		autoActivatedChainsGUIDS.RemoveAt (id);
	}
	public void AddAutoActivatedChain(Condition cond, Chain c)
	{
		autoActivatedChainsGUIDS.Add (new ConditionChain(cond, c.ChainGuid, c.StartState.stateGUID));
	}

    public List<ConditionChain> autoActivatedChainsGUIDS = new List<ConditionChain>();
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
		foreach (KeyValuePair<Condition, State> pair in autoActivatedChains)
        {
            if (pair.Key.ConditionValue)
            {
                OnParamActivation(pair.Value);
            }
        }
    }

	public Param(int Guid)
	{
		this.paramGUID = Guid;
		name = "new param";
		showing = false;
		description = "";
		activating = false;
		tags = "";
	}
}