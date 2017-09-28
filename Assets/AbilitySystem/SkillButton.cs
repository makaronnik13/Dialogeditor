using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour {

	private Ability ability;
	public Ability Ability
	{
		get
		{
			return ability;
		}
	}
	private Slider coldownSlider;
	private Button skillButton;

	public void SetAbility(Ability ability)
	{
		this.ability = ability;
		coldownSlider.fillRect.GetComponent<Image> ().sprite = ability.icon;
		coldownSlider.transform.GetChild (0).GetComponent<Image> ().sprite = ability.icon;
		if (ability.GetType () == typeof(ActiveAbility)) {
			coldownSlider.value = StatsManager.Instance.GetValue (((ActiveAbility)ability).cooldown);
			skillButton.onClick.AddListener (()=>{
				Activate();
			});
			StatsManager.Instance.onValueChanged.AddListener (()=>{
				OnValueChanged();
			});
		} else 
		{
			coldownSlider.value = 1;
			coldownSlider.maxValue = 1;
		}
	}

	public void Awake()
	{
		skillButton = GetComponent<Button> ();
		coldownSlider = GetComponentInParent <Slider> ();
	}

	public void Update()
	{
		if (coldownSlider.value < coldownSlider.maxValue) {
			coldownSlider.value += Time.deltaTime;
			skillButton.interactable = false;
		} else 
		{
			if (StatsManager.Instance.GetValue (((ActiveAbility)ability).value.stat) >= -((ActiveAbility)ability).value.value) {
				skillButton.interactable = true;
			} else {
				skillButton.interactable = false;
			}
		}
	}

	public void Activate()
	{
		skillButton.interactable = false;
		StatsManager.Instance.ChangeParam (((ActiveAbility)ability).value);
		coldownSlider.value = coldownSlider.minValue;
		((ActiveAbility)ability).Activate ();
	}

	private void OnValueChanged()
	{
		coldownSlider.minValue = 0;
		coldownSlider.maxValue = StatsManager.Instance.GetValue (((ActiveAbility)ability).cooldown);
		if(coldownSlider.value >= coldownSlider.maxValue)
		{
			if (StatsManager.Instance.GetValue (((ActiveAbility)ability).value.stat) >= -((ActiveAbility)ability).value.value) {
				skillButton.interactable = true;
			} else {
				skillButton.interactable = false;
			}
		}
	}

	public void Remove()
	{
		Destroy(coldownSlider.gameObject);
	}
}
