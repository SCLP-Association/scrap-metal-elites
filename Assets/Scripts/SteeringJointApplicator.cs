using System;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "steeringJoint", menuName = "Joints/Steering")]
public class SteeringJointApplicator : JointApplicator {
    public float steeringTorque;
    public float steeringDamper;
    public float maxTurnAngle;

    protected override Joint ApplyJoint(GameObject rigidbodyGo, PartConfig config, GameObject target) {
        // add fixed joint component to target
        var joint = rigidbodyGo.AddComponent<HingeJoint>();

        // add steering actuator
        var steering = rigidbodyGo.AddComponent<SteeringActuator>();
        if (config != null && config.Get<bool>(ConfigTag.SteeringReverse)) {
            steering.reverse = true;
        } else {
            steering.reverse = false;
        }
        steering.maxTurnAngle = maxTurnAngle;

        // apply hinge properties
        joint.axis = new Vector3(0,1,0);
        var limits = joint.limits;
        limits.min = -maxTurnAngle;
        limits.max = maxTurnAngle;
        joint.limits = limits;
        joint.useLimits = true;
        var hingeSpring = joint.spring;
        hingeSpring.spring = steeringTorque;
        hingeSpring.damper = steeringDamper;
        hingeSpring.targetPosition = 0;
        joint.spring = hingeSpring;
        joint.useSpring = true;

        return joint;
    }
}
