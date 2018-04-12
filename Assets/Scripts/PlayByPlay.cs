using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayByPlay : MonoBehaviour {

	private BotHealth[] hp;		// watch HP
	private AIController[] ai;	// watch Mood

	void Start () {

		// we don't look for bots here because
		// they may not have spawned yet
	
	}

	public void someoneDied(GameObject who) { // TODO: determine "by whom"
		
		Debug.Log ("Play-by-play announcer: " + who.name + " BITES THE DUST!");

	}

	public void someoneTookDamage(int quantity) { // TODO: who?

		Debug.Log ("Play-by-play announcer: " + quantity + " DAMAGE!");

	}

	// Update is called once per frame
	void Update () {
	
		if (Time.time < 3.0f) { // wait for bots to be spawned 
			return; // TODO: search each frame until they appear instead
		}

		if (hp == null) { // needs to init?

			Debug.Log ("Play-by-play announcer: HERE COME THE COMPETITORS...");

			// enumerate them all
			hp = FindObjectsOfType<BotHealth> ();
			ai = FindObjectsOfType<AIController> ();

			// listen to events
			for(int num=0; num < hp.Length; num++) {
				hp[num].onDeath.AddListener (someoneDied);
				hp[num].onChangePercent.AddListener (someoneTookDamage);
			}

			Debug.Log ("Play-by-play announcer: I found " + hp.Length + " competitors!");

		}

	}
}
