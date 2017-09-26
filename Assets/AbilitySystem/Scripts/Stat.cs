using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "RPG/Stat")]
[System.Serializable]
public class Stat : ScriptableObject
{
	public int id;
	public string name = "stat";
	public StatString description;
	public float maxValue = 1;
	public float minValue = 0;
	public float defaultValue = 0.5f;
	public bool evaluated = false;
	public ModificatorStruct evaluationStruct;
}
