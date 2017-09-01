using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[RequireComponent(typeof(AudioSource))]
public class PersonDialog : MonoBehaviour {

	public Path[] pathes;
	public PathEvent[] pathEvents;
	public Dictionary<Path, PathEvent> pathEventsList = new Dictionary<Path, PathEvent>();

	public Dictionary<Path, PathEvent> PathEventsList
	{
		get
		{
			Dictionary<Path, PathEvent> newPathEvents = new Dictionary<Path, PathEvent> ();
			for(int i = 0; i<pathes.Length;i++)
			{
				newPathEvents.Add (pathes[i], (PathEvent)pathEvents[i]);
			}
			pathEventsList = newPathEvents;
			return newPathEvents;
		}
	}
    [HideInInspector]
    public PathGame game;
	[SerializeField]
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

	public void Start()
	{
		DialogPlayer.Instance.onPathGo+=new DialogPlayer.PathEventHandler(InvokeEvent);
	}

	public void Talk()
	{
		DialogPlayer.Instance.PlayState (personChain.StartState);
	}

	public void InvokeEvent(Path p)
	{
		if(pathEventsList.ContainsKey(p))
		{
			pathEventsList [p].Invoke ();
		}
	}
}
