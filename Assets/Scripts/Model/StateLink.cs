﻿using UnityEngine;

[System.Serializable]
public class StateLink
{
    public int chainGUID;
    public int stateGUID;
    public Rect position;

    public StateLink(Chain chain)
    {
        this.chainGUID = chain.ChainGuid;
        this.stateGUID = chain.StartState.stateGUID;
        position = new Rect(0, 0, 100, 30);
    }
}