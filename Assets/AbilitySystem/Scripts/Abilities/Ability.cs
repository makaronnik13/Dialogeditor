using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "RPG/Abilities/Simple")]
[System.Serializable]
public class Ability : StatWithId
{
	public int startLevel = 0;
	public int maxLevel = 10;
	public Sprite icon;
	public ModificatorCondition upgradeCondition;
    public SkillCondition upgradeSkillCondition;
    public List<StatValue> cost;
}
