using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "RPG/Abilities/Simple")]
[System.Serializable]
public class Ability : ScriptableObject {
	public int maxLevel = 10;
	public Sprite icon;
	public string name = "ability";
	public ModificatorCondition description;
	public ModificatorCondition upgradeCondition;
    public SkillCondition upgradeSkillCondition;
	public List<StatValue> cost = new List<StatValue>();
}
