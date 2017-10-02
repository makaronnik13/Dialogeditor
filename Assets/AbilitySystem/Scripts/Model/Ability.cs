using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Ability", menuName = "RPG/Ability")]
[System.Serializable]
public class Ability : StatWithId
{
	public int startLevel = 0;
	public int maxLevel = 10;
	public Sprite icon;
	public ModificatorCondition upgradeCondition;
    public List<StatValue> cost;

	//active
	public bool activating;
	public ModificatorCondition cooldown;
	public StatValue value;
	public virtual void Activate()
	{

	}

	//multy
	//public bool multy;
	//public List<Ability> abilities = new List<Ability>();

	//aimed
	public bool aimed;
	public List<string> aimTags = new List<string>();

	//modificator
	public bool withModificator;
	public StatModificator modivicator;

	//aura
	public bool withAura;
	public StatsManager auraModificator;
	public float radius = 5;
	public List<string> auraTags = new List<string>();

	//auto active
	public bool autoActivated;
	public UnityEvent onActivate;
	public virtual void Awake()
	{
		if(autoActivated)
		{
			onActivate.AddListener (()=>{
				Activate();
			});
		}
	}

	//projectile
	public GameObject projectile;

	//summon
	//public GameObject summoningObject;
}
