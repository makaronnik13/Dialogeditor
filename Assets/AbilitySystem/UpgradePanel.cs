using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanel : Singleton<UpgradePanel> {

	private List<SkillUpgradeButton> upgradeButtons = new List<SkillUpgradeButton> ();

	public void SetUpgradingAbilities(List<PlayerSkill> skills)
	{
		foreach(SkillUpgradeButton su in upgradeButtons)
		{
			su.Remove ();
		}
	}
}
