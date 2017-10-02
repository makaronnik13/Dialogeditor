using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillManager : Singleton<SkillManager>{

    public UnityEvent onSkillLevelChanged;
    private List<Ability> skills;

	public void AddSkill(Ability ability)
    {
        skills.Add(ability);
    }

    public void UpSkillLevel(Ability skill)
    {
        StatsManager.Instance.SetParam(skill, StatsManager.Instance.GetValue(skill)+1);
        onSkillLevelChanged.Invoke();
    }

    public bool CheckSkillUpgradeCondition(SkillCondition upgradeSkillCondition)
    {
        List<float> skillsLevels = new List<float>();
        foreach(Ability ps in skills)
        {
            skillsLevels.Add(StatsManager.Instance.GetValue(ps));
        }
        return ExpressionSolver.CalculateBool(upgradeSkillCondition.conditionString, skillsLevels);
    }
}
