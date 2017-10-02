using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsPanel : Singleton<SkillsPanel> {
	private GameObject skillButtonPrefab;

	void Awake()
	{
		skillButtonPrefab = Resources.Load ("Prefabs/SkillButton") as GameObject;
	}

	public void AddSkill(Ability ps)
	{
		SkillButton newSkillButton = Instantiate (skillButtonPrefab, transform).GetComponentInChildren<SkillButton>();
		newSkillButton.SetAbility (ps);
	}

	public void RemoveSkill(Ability ps)
	{
		foreach(SkillButton sb in GetComponentsInChildren<SkillButton>())
		{
			if(sb.Ability == ps)
			{
				sb.Remove ();
			}
		}
	}

    public bool ContainSkill(Ability skill)
    {
        foreach (SkillButton sb in GetComponentsInChildren<SkillButton>())
        {
            if (sb.Ability == skill)
            {
                return true;
            }
        }
        return false;
    }
}
