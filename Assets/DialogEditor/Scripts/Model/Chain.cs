using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Chain : ScriptableObject
{
    private PathGame game;
    public PathGame Game
    {
        get
        {
           return game;
        }
    }

    public string dialogName
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
            game.Dirty = true;
        }
    }
    [HideInInspector]
    public State StartState;
    [HideInInspector]
    public State inspectedState;
    [HideInInspector]
    public List<State> states = new List<State>();
    [HideInInspector]
    public List<StateLink> links = new List<StateLink>();

    public StateLink AddStateLink()
    {
        StateLink sl = CreateInstance<StateLink>();
        sl.Init(this);
        GuidManager.GetGameByChain(this).Dirty = true;
        links.Add(sl);
        return sl;
    }

    public void RemoveStateLink(StateLink link)
    {
        links.Remove(link);
        GuidManager.GetGameByChain(this).Dirty = true;
    }

    public State AddState()
    {
        State newState = CreateInstance<State>();
        game.Dirty = true;
        states.Add(newState);
        newState.Init(this);
        return newState;
    }

    public void RemoveState(State state)
    {
        states.Remove(state);
        game.Dirty = true;
    }


    public void Init(PathGame game)
    {
        this.game = game;
        game.chains.Insert(0, this);
        dialogName = "New chain";
        name = dialogName;
    }
}
