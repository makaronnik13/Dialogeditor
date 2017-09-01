using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class DialogPlayer : Singleton<DialogPlayer> 
{
	public delegate void StateEventHandler(State e);
	public delegate void PathEventHandler(Path e);

	public event StateEventHandler onStateIn;
	public event PathEventHandler onPathGo;

	public void PlayState(State state)
	{
		onStateIn.Invoke (state);
	}

	public void PlayPath(Path p)
	{
		onPathGo.Invoke (p);
		PlayState (p.aimState);
	}
}
