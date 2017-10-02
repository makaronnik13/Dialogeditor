using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpgradeButton : MonoBehaviour {

	private Ability skill;
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

	public void SetSkill(Ability skill)
	{
		this.skill = skill;
        GetComponent<Image>().sprite = skill.icon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(UpgradeSkill);
        StatsManager.Instance.onValueChanged.AddListener(ValueChanged);
        SkillManager.Instance.onSkillLevelChanged.AddListener(ValueChanged);
        ValueChanged();
    }

    private void ValueChanged()
    {
        button.interactable = StatsManager.Instance.CheckCondition(skill.upgradeCondition) && StatsManager.Instance.GetValue(skill)<skill.maxLevel;
        foreach (StatValue sv in skill.cost)
        {
            if (StatsManager.Instance.GetValue(sv.stat)< sv.value)
            {
                button.interactable = false;
            }
        }

        button.interactable = button.interactable && SkillManager.Instance.CheckSkillUpgradeCondition(skill.upgradeSkillCondition);
    }

    private void UpgradeSkill()
    {
        SkillManager.Instance.UpSkillLevel(skill);
        foreach (StatValue sv in skill.cost)
        {
            StatsManager.Instance.ChangeParam(sv);
        }
        lvlField.SetActive(true);
        lvl.text = "" + StatsManager.Instance.GetValue(skill);
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
