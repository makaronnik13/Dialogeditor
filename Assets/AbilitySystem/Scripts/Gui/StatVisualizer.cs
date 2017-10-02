using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatVisualizer : MonoBehaviour {

	public Text text;
	public Slider slider;
	public bool showMax = true;
	public Stat stat;

	public void OnValueChanged()
	{
		if(slider)
		{
			slider.maxValue = stat.MaxValue;
			slider.minValue = stat.MinValue;
			slider.value = StatsManager.Instance.GetValue (stat);
		}
		if(text)
		{
			text.text = StatsManager.Instance.GetValue (stat)+"";
			if(showMax)
			{
				text.text += "/"+stat.MaxValue;
			}
		}
	}


	void Awake () 
	{
		StatsManager.Instance.onValueChanged.AddListener (()=>{
			OnValueChanged();
		});	
	}
}
