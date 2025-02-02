﻿// ----------------------------------------------------------------------------
// Unite 2017 - Game Architecture with Scriptable Objects
//
// Author: Ryan Hipple
// Date:   10/04/17
// ----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityStringEvent : UnityEvent<string> {};

public class StringEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public StringEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityStringEvent Response;

    public void Awake() {
        if (Response == null) {
            Response = new UnityStringEvent();
        }
    }

    public void SetEvent(StringEvent newEvent) {
        // unregister current event
        if (this.Event != null) {
            this.Event.UnregisterListener(this);
        }
        // register new
        if (newEvent != null) {
            this.Event = newEvent;
            this.Event.RegisterListener(this);
        }
    }

    private void OnEnable() {
        if (Event != null) {
            Event.RegisterListener(this);
        }
    }

    private void OnDisable() {
        if (Event != null) {
            Event.UnregisterListener(this);
        }
    }

    public void OnEventRaised(string message)
    {
        Response.Invoke(message);
    }
}
