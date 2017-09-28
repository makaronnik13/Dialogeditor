using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanel : Singleton<UpgradePanel> {

	private List<SkillUpgradeButton> upgradeButtons = new List<SkillUpgradeButton> ();
    private GameObject SkillUpgradeButtonPrefab;

    void Awake()
    {
        SkillUpgradeButtonPrefab = Resources.Load("Prefabs/SkillUpgradeButton") as GameObject;
    }


    public void SetUpgradingAbilities(List<PlayerSkill> skills)
	{
		foreach(SkillUpgradeButton su in upgradeButtons)
		{
			su.Remove ();
		}

        upgradeButtons.Clear();

        foreach (PlayerSkill ps in skills)
        {
            SkillUpgradeButton newSkillUpgradeButton = Instantiate(SkillUpgradeButtonPrefab, transform).GetComponent<SkillUpgradeButton>();
            newSkillUpgradeButton.SetSkill(ps);
        }
	}
}
