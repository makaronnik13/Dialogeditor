using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatModificator
{
	public List<ModificatorChanger> Changers = new List<ModificatorChanger>();
	public ModificatorCondition Condition;
}
