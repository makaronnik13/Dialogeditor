using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill 
{
	public Ability ability;
	public int level;

	public PlayerSkill(Ability ability)
	{
		this.ability = ability;
		level = ability.startLevel;
	}
}
