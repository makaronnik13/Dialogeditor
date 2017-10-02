using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillManager : Singleton<SkillManager>{

    public UnityEvent onSkillLevelChanged;
	private List<Ability> skills = new List<Ability>();

	public void AddSkill(Ability ability)
    {
        skills.Add(ability);
    }

    public void UpSkillLevel(Ability skill)
    {
        StatsManager.Instance.SetParam(skill, StatsManager.Instance.GetValue(skill)+1);
        onSkillLevelChanged.Invoke();
    }
}
