using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class State : ScriptableObject
{
    private PathGame game;

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
                ss = ss.Substring(0, Mathf.Min(10, ss.Length));
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
        game.Dirty = true;
        return newPath;
    }

    public void RemovePath(Path path)
    {
        game.Dirty = true;
        pathes.Remove(path);
        DestroyImmediate(path, true);
    }

    public void DestroyState()
    {
        int pc = pathes.Count;
        for (int i = pc - 1; i >= 0; i--)
        {
            RemovePath(pathes[i]);
        }
    }
}

