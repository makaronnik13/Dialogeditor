using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModificatorCondition 
{
	public string conditionString;
	public List<Stat> conditionStats = new List<Stat>();
}
