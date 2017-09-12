using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingStone : MonoBehaviour {

	public void Take(AvatarIKGoal bodyPart)
    {
        FindObjectOfType<IKController>().MoveTo(bodyPart, transform);
    }
}
