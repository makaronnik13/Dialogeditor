﻿using UnityEngine;

[System.Serializable]
public class StateLink : ScriptableObject
{
    private PathGame game;
    public Chain chain;
    public State _state;
    public State state
    {
        get
        {
            return _state;
        }
        set
        {
            if (_state != value)
            {
                _state = value;
                name = _state.name + "_link";
                game.Dirty = true;
            }
        }
    }
    public Rect position;

    public void Init(Chain chain)
    {
        this.game = chain.Game;
        this.chain = chain;
        state = chain.StartState;
        float z = 1;
        z = GuidManager.GetGameByChain(chain).zoom;
        position = new Rect(0, 0, 100 * z, 30 * z);
    }
}