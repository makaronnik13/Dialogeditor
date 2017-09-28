using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillManager : Singleton<SkillManager>{

    public UnityEvent onSkillLevelChanged;
    private List<PlayerSkill> skills;

	public void AddSkill(Ability ability)
    {
        skills.Add(new PlayerSkill(ability));
    }

    public void UpSkillLevel(PlayerSkill skill)
    {
        skill.level++;
        onSkillLevelChanged.Invoke();
    }

    public bool CheckSkillUpgradeCondition(SkillCondition upgradeSkillCondition)
    {
        List<float> skillsLevels = new List<float>();
        foreach(PlayerSkill ps in skills)
        {
            skillsLevels.Add(ps.level);
        }
        return ExpressionSolver.CalculateBool(upgradeSkillCondition.conditionString, skillsLevels);
    }
}
