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
		if (ability.activating) {
			coldownSlider.value = StatsManager.Instance.GetValue (ability.cooldown);
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
			skillButton.interactable = false;
			foreach(StatValue sv in ability.value){
				if (StatsManager.Instance.GetValue (sv.stat) >= -StatsManager.Instance.GetValue(sv.value)) 
			{
				skillButton.interactable = true;
			} 
				
			}
		}
	}

	public void Activate()
	{
		skillButton.interactable = false;
		foreach (StatValue sv in ability.value) {
			StatsManager.Instance.ChangeParam (sv);
		}
		coldownSlider.value = coldownSlider.minValue;
		ability.Activate ();
	}

	private void OnValueChanged()
	{
		coldownSlider.minValue = 0;
		coldownSlider.maxValue = StatsManager.Instance.GetValue (ability.cooldown);
		if(coldownSlider.value >= coldownSlider.maxValue)
		{
			skillButton.interactable = false;
			foreach(StatValue sv in ability.value){
				if (StatsManager.Instance.GetValue (sv.stat) >= - StatsManager.Instance.GetValue(sv.value)) {
				skillButton.interactable = true;
			}

			}
		}
	}

	public void Remove()
	{
		Destroy(coldownSlider.gameObject);
	}
}
