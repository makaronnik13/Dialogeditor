using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RobotJoint))]
public class RobotJointInspector : Editor {
    private RobotJoint _joint;
    private RobotJoint joint
    {
        get
        {
            if (!_joint)
            {
                _joint = (RobotJoint)target;
            }
            return _joint;
        }
    }

    private Vector3 xHandlePositionMin, xHandlePositionMax, yHandlePositionMin, yHandlePositionMax, zHandlePositionMin, zHandlePositionMax;

    private void OnEnable()
    {
        
    }

    void OnSceneGUI()
    {
        Handles.color = Color.yellow;
        Handles.DrawSphere(55, joint.transform.position, Quaternion.identity, 0.05f);

        Handles.color = Color.green;
        Handles.DrawWireArc(joint.transform.position,
                  joint.transform.parent.up,
                  Quaternion.AngleAxis(joint.minYRotation, joint.transform.parent.up)* joint.transform.parent.right,
                  joint.maxYRotation-joint.minYRotation,
                  0.5f);

      
        Handles.color = Color.red;
        Handles.DrawWireArc(joint.transform.position,
                  joint.transform.parent.right,
                  Quaternion.AngleAxis(joint.minXRotation, joint.transform.parent.right) * joint.transform.parent.up,
                  joint.maxXRotation - joint.minXRotation,
                  0.5f);
        Handles.color = Color.blue;
        Handles.DrawWireArc(joint.transform.position,
                  joint.transform.parent.forward,
                  Quaternion.AngleAxis(joint.minZRotation, joint.transform.parent.forward) * joint.transform.parent.up,
                  joint.maxZRotation - joint.minZRotation,
                  0.5f);

        Handles.color = Color.green;
        yHandlePositionMin = joint.transform.position + Quaternion.AngleAxis(joint.minYRotation, joint.transform.parent.up) * (joint.transform.parent.right * 0.5f);
        yHandlePositionMax = joint.transform.position + Quaternion.AngleAxis(joint.maxYRotation, joint.transform.parent.up) * (joint.transform.parent.right * 0.5f);
        yHandlePositionMin = Handles.FreeMoveHandle(yHandlePositionMin, joint.transform.parent.rotation, 0.02f, Vector3.one*0.5f, Handles.SphereHandleCap);
        yHandlePositionMax = Handles.FreeMoveHandle(yHandlePositionMax, joint.transform.parent.rotation, 0.02f, Vector3.one * 0.5f, Handles.SphereHandleCap);

        //joint.minYRotation = Vector3.Angle(joint.transform.parent.right, new Vector3((joint.transform.parent.position - Handles.FreeMoveHandle(yHandlePositionMin, joint.transform.parent.rotation, 0.02f, Vector3.one*0.5f, Handles.SphereHandleCap)).x, 0));
        
        Handles.color = Color.red;
        xHandlePositionMin = joint.transform.position + Quaternion.AngleAxis(joint.minXRotation, joint.transform.parent.right) * (joint.transform.parent.up * 0.5f);
        xHandlePositionMax = joint.transform.position + Quaternion.AngleAxis(joint.maxXRotation, joint.transform.parent.right) * (joint.transform.parent.up * 0.5f);
        xHandlePositionMin = Handles.FreeMoveHandle(xHandlePositionMin, joint.transform.parent.rotation, 0.02f, Vector3.one * 0.5f, Handles.SphereHandleCap);
        xHandlePositionMax = Handles.FreeMoveHandle(xHandlePositionMax, joint.transform.parent.rotation, 0.02f, Vector3.one * 0.5f, Handles.SphereHandleCap);

        Handles.color = Color.blue;
        zHandlePositionMin = joint.transform.position + Quaternion.AngleAxis(joint.minZRotation, joint.transform.parent.forward) * (joint.transform.parent.up * 0.5f);
        zHandlePositionMax = joint.transform.position + Quaternion.AngleAxis(joint.maxZRotation, joint.transform.parent.forward) * (joint.transform.parent.up * 0.5f);
        zHandlePositionMin = Handles.FreeMoveHandle(zHandlePositionMin, joint.transform.parent.rotation, 0.02f, Vector3.one * 0.5f, Handles.SphereHandleCap);
        zHandlePositionMax = Handles.FreeMoveHandle(zHandlePositionMax, joint.transform.parent.rotation, 0.02f, Vector3.one * 0.5f, Handles.SphereHandleCap);
        
    }

    //public override void OnInspectorGUI()
    //{

    //}
}
