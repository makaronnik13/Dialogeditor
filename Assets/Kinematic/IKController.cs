using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour {
    public bool IkActive = true;
    public Transform RightLeg, LeftLeg, RightArm, LeftArm;
    private Transform RightLegHandle, LeftLegHandle, RightArmHandle, LeftArmHandle;

	// Use this for initialization
	void Start () {
        IKCtrl ik = gameObject.AddComponent<IKCtrl>();
        GameObject aim = new GameObject();
        aim.transform.position = RightLeg.transform.position;
        aim.transform.rotation = RightLeg.transform.rotation;
        RightLegHandle = aim.transform;
        ik.aim = RightLegHandle;
        ik.bodyPart = AvatarIKGoal.RightFoot;
        ik.ikActive = true;

        ik = gameObject.AddComponent<IKCtrl>();
        aim = new GameObject();
        aim.transform.position = LeftLeg.transform.position;
        aim.transform.rotation = LeftLeg.transform.rotation;
        LeftLegHandle = aim.transform;
        ik.aim = LeftLegHandle;
        ik.bodyPart = AvatarIKGoal.LeftFoot;
        ik.ikActive = true;

        ik = gameObject.AddComponent<IKCtrl>();
        aim = new GameObject();
        aim.transform.position = RightArm.transform.position;
        aim.transform.rotation = RightArm.transform.rotation;
        RightArmHandle = aim.transform;
        ik.aim = RightArmHandle;
        ik.bodyPart = AvatarIKGoal.RightHand;
        ik.ikActive = true;

        ik = gameObject.AddComponent<IKCtrl>();
        aim = new GameObject();
        aim.transform.position = LeftArm.transform.position;
        aim.transform.rotation = LeftArm.transform.rotation;
        LeftArmHandle = aim.transform;
        ik.aim = LeftArmHandle;
        ik.bodyPart = AvatarIKGoal.LeftHand;
        ik.ikActive = true;
    }


    private void Update()
    {
        if (GetComponent<IKCtrl>().ikActive!=IkActive)
        {
            if (IkActive)
            {
                Off();
            }
            else
            {
                On();
            }
        }
    }

    public void MoveTo(Transform goal, Transform aim, float time = 1)
    {
        if (goal == RightArm)
        {
            goal = RightArmHandle;
        }
        if (goal == LeftArm)
        {
            goal = LeftArmHandle;
        }
        if (goal == RightLeg)
        {
            goal = RightLegHandle;
        }
        if (goal == LeftLeg)
        {
            goal = LeftLegHandle;
        }

        
        
        StartCoroutine(MoveToC(goal, aim, time));
    }

    public void MoveTo(AvatarIKGoal goal, Transform aim, float time = 1)
    {
        switch (goal)
        {
            case AvatarIKGoal.LeftFoot:
                MoveTo(LeftLeg, aim, time);
                break;
            case AvatarIKGoal.RightFoot:
                MoveTo(RightLeg, aim, time);
                break;
            case AvatarIKGoal.LeftHand:
                MoveTo(LeftArm, aim, time);
                break;
            case AvatarIKGoal.RightHand:
                MoveTo(RightArm, aim, time);
                break;
        }
    }


    private Vector3 CalculateCenterOfMass()
    {
        Vector3 center = Vector3.zero;
        float mass = 0;

        foreach (BodyPart bp in GetComponentsInChildren<BodyPart>())
        {
            center += bp.transform.position * bp.weight;
            mass += bp.weight;
        }
        center /= GetComponentsInChildren<BodyPart>().Length*0.5f;

        return center/mass;
    }

    private IEnumerator MoveToC(Transform part, Transform aim, float time)
    {
        if (aim == null || (Vector3.Distance(part.position, aim.position) < 0.1f && Vector3.Angle(part.transform.rotation.eulerAngles, aim.transform.rotation.eulerAngles) < 0.1f))
        {
            yield return null;
        }
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            part.transform.position = Vector3.Lerp(part.transform.position, aim.transform.position, (elapsedTime/time));
            part.transform.rotation = Quaternion.Lerp(part.transform.rotation, aim.transform.rotation, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, CalculateCenterOfMass(), (elapsedTime / time));
            yield return new WaitForEndOfFrame();
        }
    }

    [ContextMenu("activate")]
    public void On()
    {
        IkActive = true;
        foreach (IKCtrl ik in GetComponents<IKCtrl>())
        {
            ik.ikActive = true;
        }
    }

    [ContextMenu("deactivate")]
    public void Off()
    {
        IkActive = false;
        StopCoroutine("MoveTo");
        foreach (IKCtrl ik in GetComponents<IKCtrl>())
        {
            ik.ikActive = false;
        }
    }
}
