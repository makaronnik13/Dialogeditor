using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AutoActivatedAbility : ActiveAbility {

	public UnityEvent onActivate;

	public virtual void Awake()
	{
		onActivate.AddListener (()=>{
			Activate();
		});
	}
}
