using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(AudioSource))]
public class PersonDialog : MonoBehaviour
{
    private Chain _personChain;
    public Chain personChain
    {
        set
        {
            _personChain = value;
        }
        get
        {
            return _personChain;
        }
    }

    public bool playing;
    public Path[] pathes;
    public PathEvent[] pathEvents;
    public Dictionary<Path, PathEvent> pathEventsList = new Dictionary<Path, PathEvent>();
    public Dictionary<Path, PathEvent> PathEventsList
    {
        get
        {
            Dictionary<Path, PathEvent> newPathEvents = new Dictionary<Path, PathEvent>();
            for (int i = 0; i < pathes.Length; i++)
            {
                newPathEvents.Add(pathes[i], (PathEvent)pathEvents[i]);
            }
            pathEventsList = newPathEvents;
            return newPathEvents;
        }
    }
    [HideInInspector]
    public PathGame game;
    [SerializeField]

    public void Start()
    {
        DialogPlayer.Instance.onPathGo += new DialogPlayer.PathEventHandler(InvokeEvent);
    }

    public void Talk()
    { 
        playing = true;
        DialogPlayer.Instance.PlayState(personChain.StartState, this);
    }

    public void InvokeEvent(Path p)
    {
        if (!personChain.states.Contains(p.aimState))
        {
            personChain = GuidManager.GetChainByState(p.aimState);
        }
        if (pathEventsList.ContainsKey(p))
        {
            pathEventsList[p].Invoke();
        }
    }
}
