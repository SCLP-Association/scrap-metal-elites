using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Simple Bot Drive Controller
// Requires IMovement component in same game object
public class BotBrain : MonoBehaviour {
    protected IMovement mover;
    protected IActuator weapon;


    [Header("State Variables")]
    public BotRuntimeSet botList;
    public GameRecordEvent eventChannel;

    protected bool botAlive = true;
    protected bool flipped = false;
    protected bool controlsActive = true;

    void OnBotDeath(GameObject from) {
        botAlive = false;
    }

    public virtual void EnableControls() {
        controlsActive = true;
    }
    public virtual void DisableControls() {
        controlsActive = false;
    }

	// Use this for initialization
	void Start () {
        mover = GetComponent<IMovement>();
        weapon = GetComponent<IActuator>();
        if (eventChannel == null) {
            eventChannel = PartUtil.GetEventChannel(gameObject);
            Debug.Log("eventChannel: " + eventChannel);
        }
        // determine when bot flips
        StartCoroutine(DetectFlip());

        // determine when bot dies
        var botHealth = GetComponent<BotHealth>();
        if (botHealth != null) {
            botHealth.onDeath.AddListener(OnBotDeath);
        }
        if (controlsActive) EnableControls();
	}

    public void OnGameRecord(GameRecord record) {
        Debug.Log(record.ToString());
    }

    IEnumerator DetectFlip() {
        while (botAlive) {
            var currentFlip = Vector3.Angle(Vector3.up, transform.up) > 90;
            if (currentFlip != flipped) {
                flipped = currentFlip;
                if (eventChannel != null) {
                    eventChannel.Raise(GameRecord.BotFlipped(gameObject));
                }
            }
            // wait until next frame;
            yield return null;
        }
    }

}
