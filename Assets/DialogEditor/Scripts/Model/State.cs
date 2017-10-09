using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class State : ScriptableObject
{
	#if UNITY_EDITOR
	public string shortName;
	#endif
    private PathGame game;
    public PathGame Game
    {
        get
        {
            return game;
        }
        set
        {
            game = value;
        }
    }
    private Chain chain;
    public Chain Chain
    {
        get
        {
            return chain;
        }
        set
        {
            chain = value;
        }
    }

    public string _description;
    public string description
    {
        get
        {
            return _description;
        }
        set
        {
            if (value != "")
            {
                string ss = value.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
                ss = ss.Substring(0, 20);
                if (name != ss)
                {
                    name = ss;
                    game.Dirty = true;
                }
            }
            _description = value;
        }
    }
    public List<Path> pathes = new List<Path>();
    public Rect position;
    public AudioClip sound;

    public void Init(Chain chain)
    {
        this.chain = chain;
        game = chain.Game;
        description = "";
        float z = 1;
        z =game.zoom;
        position = new Rect(300, 300, 208 * z, 30 * z);
    }

    public Path AddPath()
    {
        Path newPath = CreateInstance<Path>();
        newPath.Game = game;
        pathes.Add(newPath);
        Game.Dirty = true;
        return newPath;
    }

    public void RemovePathWithoutDestroy(Path path)
    {
        game.Dirty = true;
        pathes.Remove(path);
    }

    public void RemovePath(Path path)
    {
        game.Dirty = true;
        pathes.Remove(path);
    }

}

