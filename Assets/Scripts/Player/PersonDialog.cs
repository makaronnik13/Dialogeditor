using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[RequireComponent(typeof(AudioSource))]
public class PersonDialog : MonoBehaviour {

	public Path[] pathes;
	public UnityEvent[] pathEvents;

	public Dictionary<Path, UnityEvent> pathEventsList
	{
		get
		{
			Dictionary<Path, UnityEvent> newPathEvents = new Dictionary<Path, UnityEvent> ();
			for(int i = 0; i<pathes.Length;i++)
			{
				newPathEvents.Add (pathes[i], pathEvents[i]);
			}
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
			currentState = value.StartState;
		}
		get
		{
			return _personChain;
		}
	}

	[SerializeField]
	private State currentState;

	public void Talk()
	{
		DialogGui.Instance.SetGame (game);
		DialogGui.Instance.SetActions (pathEventsList);
		DialogGui.Instance.ShowText(currentState);
	}
}
