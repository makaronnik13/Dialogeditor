using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability : ScriptableObject {
	public int startLevel = 0;
	public int maxLevel = 10;
	public Sprite icon;
	public string name = "ability";
	public StatString description;
	public ModificatorCondition upgradeCondition;
}
