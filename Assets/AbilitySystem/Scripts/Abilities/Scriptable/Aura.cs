using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : PassiveAbility {

	public Modificator modificator;
	public float radius = 5;
	public List<AimTag> tags = new List<AimTag>();
}
