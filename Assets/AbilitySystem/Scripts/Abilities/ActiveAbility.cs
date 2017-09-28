using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "RPG/Abilities/Active")]
[System.Serializable]
public class ActiveAbility : Ability {

	public ModificatorCondition cooldown;
	public StatValue value;

	public virtual void Activate()
	{
		
	}
}
