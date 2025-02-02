﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorActuator : MonoBehaviour, IMotorActuator {
    public float forwardDrive {
        get {
            return _forwardDrive;
        }
        set {
            _forwardDrive = value;
        }
    }
    public bool left {
        get {
            return isLeft;
        }
    }

    private Rigidbody rb;
    private GameObject rootGo;
    private float _forwardDrive = 0.0f;
    public bool isLeft = false;
    public float maxTorque;
    public float maxSpeed;
    public bool reverse = false;
    public bool motorOn = false;
    public AudioEvent motorSfx;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rootGo = PartUtil.GetRootGo(gameObject);
    }

    void rbMotor() {
        if (rb == null) return;
        if (Mathf.Approximately(_forwardDrive, 0)) {
            if (motorOn && motorSfx != null) {
                motorSfx.Stop(AudioManager.GetInstance().GetEmitter(rootGo, motorSfx));
            }
            motorOn = false;
            return;
        }
        if (!motorOn && motorSfx != null) {
            //motorSfx.Play(audio);
            motorSfx.Play(AudioManager.GetInstance().GetEmitter(rootGo, motorSfx));
        }
        motorOn = true;
        var f = maxTorque * _forwardDrive;
        if (reverse) {
            f = -f;
        }
        rb.maxAngularVelocity = maxSpeed;
        rb.AddTorque(transform.right * f);
    }

    public void FixedUpdate() {
        rbMotor();
    }

}
