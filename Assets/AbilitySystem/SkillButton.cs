using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour {

	public ActiveAbility ability;
	private Slider coldownSlider;
	private Button skillButton;

	public void Awake()
	{
		skillButton = GetComponent<Button> ();
		coldownSlider = GetComponentInChildren <Slider> ();
		coldownSlider.value = StatsManager.Instance.GetValue(ability.cooldown);
		skillButton.onClick.AddListener (()=>{
			Activate();
		});
		StatsManager.Instance.onValueChanged.AddListener (()=>{
			OnValueChanged();
		});
		coldownSlider.fillRect.GetComponent<Image> ().sprite = ability.icon;
		coldownSlider.transform.GetChild (0).GetComponent<Image> ().sprite = ability.icon;
	}

	public void Update()
	{
		if(coldownSlider.value < coldownSlider.maxValue)
		{
			coldownSlider.value += Time.deltaTime;
			skillButton.interactable = false;
		}
	}

	public void Activate()
	{
		Debug.Log ("activate");
		skillButton.interactable = false;
		StatsManager.Instance.ChangeParams (ability.value);
		coldownSlider.value = coldownSlider.minValue;
		ability.Activate ();
	}

	private void OnValueChanged()
	{
		coldownSlider.minValue = 0;
		coldownSlider.maxValue = StatsManager.Instance.GetValue (ability.cooldown);
		if(coldownSlider.value >= coldownSlider.maxValue)
		{
			if (StatsManager.Instance.GetValue (ability.value.stat) >= ability.value.value) {
				skillButton.interactable = true;
			} else {
				skillButton.interactable = false;
			}
		}
	}
}
