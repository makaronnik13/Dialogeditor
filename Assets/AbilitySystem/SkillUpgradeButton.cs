using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpgradeButton : MonoBehaviour {

	private PlayerSkill skill;
	private Text lvl;
	private GameObject lvlField;

	void Awake()
	{
		lvlField = transform.GetChild (0).gameObject;
		lvl = GetComponentInChildren<Text> ();
		lvlField.SetActive (false);
	}

	public void SetSkill(PlayerSkill skill)
	{
		this.skill = skill;
	}

	public void Remove()
	{
		Destroy(gameObject);
	}

	public void Update()
	{
		
	}
}
