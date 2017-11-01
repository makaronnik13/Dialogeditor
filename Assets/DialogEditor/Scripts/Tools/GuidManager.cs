using System;
using System.Collections.Generic;
using UnityEngine;

public class GuidManager
{
    private static List<PathGame> games;
    private static List<PathGame> Games
    {
        get
        {
            if (games == null)
            {
                games = new List<PathGame>();
                foreach (PathGame pg in Resources.FindObjectsOfTypeAll<PathGame>())
                {
                    games.Add(pg);
                }
            }
            return games;
        }
    }

    internal static int GetStatGuid()
    {
        int r = UnityEngine.Random.Range(0, Int32.MaxValue);

        List<int> usingGuids = new List<int>();
        foreach (Stat stat in Resources.FindObjectsOfTypeAll<Stat>())
        {
            usingGuids.Add(stat.id);
        }
        foreach (Ability ability in Resources.FindObjectsOfTypeAll<Ability>())
        {
            usingGuids.Add(ability.id);
        }

        if (usingGuids.Contains(r))
        {
            return GetStatGuid();
        }

        return r;
    }

    public static int GetItemGUID()
    {
        int r = UnityEngine.Random.Range(0, Int32.MaxValue);


        foreach (PathGame inspectedgame in Games)
        {
            foreach (Param p in inspectedgame.parameters)
            {
                if (p.paramGUID == r)
                {
                    return GetItemGUID();
                }
            }
        }
        return r;
    }
    public static PathGame GetGameByParam(Param param)
    {
        foreach (PathGame inspectedgame in Games)
        {
            foreach (Param p in inspectedgame.parameters)
            {
                if (p.paramGUID == param.paramGUID)
                {
                    return inspectedgame;
                }
            }
        }
        return null;
    }

    public static int GetStateGuid()
    {
        int r = UnityEngine.Random.Range(0, Int32.MaxValue);

        foreach (PathGame inspectedgame in Games)
        {
            foreach (Chain c in inspectedgame.chains)
            {
                foreach (State s in c.states)
                {
                    if (s.Guid == r)
                    {
                        return GetStateGuid();
                    }
                }
            }
        }
        return r;
    }

    public static PathGame GetGameByState(State state)
    {
        return GetGameByChain(GetChainByState(state));
    }

    public static State GetStateByGuid(int guid)
    {
        foreach (PathGame inspectedgame in Games)
        {
            foreach (Chain c in inspectedgame.chains)
            {
                foreach (State s in c.states)
                {
                    if (s.Guid == guid)
                    {
                        return s;
                    }
                }
            }
        }
        return null;
    }

    public static PathGame GetGameByChain(Chain personChain)
    {
        foreach (PathGame inspectedgame in Games)
        {
            foreach (Chain c in inspectedgame.chains)
            {
                if (personChain == c)
                {
                    return inspectedgame;
                }
            }
        }
        return null;
    }
    public static PathGame GetGameByPath(Path p)
    {
        foreach (PathGame game in Games)
        {
            foreach (Chain chain in game.chains)
            {
                foreach (State s in chain.states)
                {
                    if (s.pathes.Contains(p))
                    {
                        return game;
                    }
                }
            }
        }
        return null;
    }

    public static StatWithId GetStatByGuid(int guid)
    {
        foreach (Stat stat in Resources.FindObjectsOfTypeAll<StatWithId>())
        {
            if (stat.Id == guid)
            {
                return stat;
            }
        }
        return null;
    }

    public static Param GetItemByGuid(int aimParamGuid)
    {
        foreach (PathGame inspectedgame in Games)
        {
            foreach (Param p in inspectedgame.parameters)
            {
                if (p.paramGUID == aimParamGuid)
                {
                    return p;
                }
            }
        }
        return null;
    }
    public static Chain GetChainByState(State state)
    {
        foreach (PathGame inspectedgame in Games)
        {
            foreach (Chain c in inspectedgame.chains)
            {
                foreach (State s in c.states)
                {
                    if (state == s)
                    {
                        return c;
                    }
                }
            }
        }
        return null;
    }
    public static State GetStateByPath(Path activeObject)
    {
        foreach (PathGame inspectedgame in Games)
        {
            foreach (Chain c in inspectedgame.chains)
            {
                foreach (State s in c.states)
                {
                    foreach (Path p in s.pathes)
                    {
                        if (p == activeObject)
                        {
                            return s;
                        }
                    }
                }
            }
        }
        return null;
    }

    public static Chain GetChainByPath(Path path)
    {
        foreach (PathGame game in games)
        {
            foreach (Chain c in game.chains)
            {
                if (c.states.Contains(GetStateByPath(path)))
                {
                    return c;
                }
            }
        }
        return null;
    }
}