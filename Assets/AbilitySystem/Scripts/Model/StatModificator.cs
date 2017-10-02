using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModificator : ScriptableObject
{
	public List<ModificatorChanger> Changers = new List<ModificatorChanger>();
	public ModificatorCondition Condition;
}
