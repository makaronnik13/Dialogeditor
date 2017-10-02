using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeInitializator : MonoBehaviour {

    public Ability[] playerSkills; 

	// Use this for initialization
	void Start () {
        foreach (Ability ab in playerSkills)
        {
            SkillManager.Instance.AddSkill(ab);
        }	
	}
	
}
