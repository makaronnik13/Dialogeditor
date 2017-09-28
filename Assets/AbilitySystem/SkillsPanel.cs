using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsPanel : Singleton<SkillsPanel> {
	private GameObject skillButtonPrefab;

	void Awake()
	{
		skillButtonPrefab = Resources.Load ("Prefabs/SkillButton") as GameObject;
	}

	public void AddSkill(PlayerSkill ps)
	{
		SkillButton newSkillButton = Instantiate (skillButtonPrefab, transform).GetComponentInChildren<SkillButton>();
		newSkillButton.SetAbility (ps.ability);
	}

	public void RemoveSkill(PlayerSkill ps)
	{
		foreach(SkillButton sb in GetComponentsInChildren<SkillButton>())
		{
			if(sb.Ability == ps.ability)
			{
				sb.Remove ();
			}
		}
	}

    public bool ContainSkill(PlayerSkill skill)
    {
        foreach (SkillButton sb in GetComponentsInChildren<SkillButton>())
        {
            if (sb.Ability == skill.ability)
            {
                return true;
            }
        }
        return false;
    }
}
