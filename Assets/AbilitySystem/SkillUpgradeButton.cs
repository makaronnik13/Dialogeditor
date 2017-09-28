using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpgradeButton : MonoBehaviour {

	private PlayerSkill skill;
	private Text lvl;
	private GameObject lvlField;
    private Button button;

	void Awake()
	{
		lvlField = transform.GetChild (0).gameObject;
		lvl = GetComponentInChildren<Text> ();
		lvlField.SetActive (false);
        button = GetComponent<Button>();
	}

	public void SetSkill(PlayerSkill skill)
	{
		this.skill = skill;
        GetComponent<Image>().sprite = skill.ability.icon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(UpgradeSkill);
        StatsManager.Instance.onValueChanged.AddListener(ValueChanged);
        SkillManager.Instance.onSkillLevelChanged.AddListener(ValueChanged);
        ValueChanged();
    }

    private void ValueChanged()
    {
        button.interactable = StatsManager.Instance.CheckCondition(skill.ability.upgradeCondition) && skill.level<skill.ability.maxLevel;
        foreach (StatValue sv in skill.ability.cost)
        {
            if (StatsManager.Instance.GetValue(sv.stat)< sv.value)
            {
                button.interactable = false;
            }
        }

        button.interactable = button.interactable && SkillManager.Instance.CheckSkillUpgradeCondition(skill.ability.upgradeSkillCondition);
    }

    private void UpgradeSkill()
    {
        SkillManager.Instance.UpSkillLevel(skill);
        foreach (StatValue sv in skill.ability.cost)
        {
            StatsManager.Instance.ChangeParam(sv);
        }
        lvlField.SetActive(true);
        lvl.text = "" + skill.level;
        ValueChanged();
        if (!SkillsPanel.Instance.ContainSkill(skill))
        {
            SkillsPanel.Instance.AddSkill(skill);
        }
    }

    public void Remove()
	{
		Destroy(gameObject);
	}
}
