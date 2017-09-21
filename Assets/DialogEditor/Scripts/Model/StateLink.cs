﻿using UnityEngine;

[System.Serializable]
public class StateLink: ScriptableObject
{
    public Chain chain;

    [SerializeField]
    private State _state;
	public State state
    {
        get
        {
            return _state;
        }
        set
        {
            if (_state!=value)
            {
                _state = value;
                name = _state.name+"_link";
                GuidManager.getGameByChain(GuidManager.GetChainByState(state)).Dirty = true;
            }
        }
    }
    public Rect position;

    public void Init(Chain chain)
    {
        this.chain= chain;
        this.state = chain.StartState;
        float z = 1;
        if (GuidManager.getGameByChain(chain))
        {
            z = GuidManager.getGameByChain(chain).zoom;
        }
        position = new Rect(0, 0, 100*z, 30*z);
    }
}