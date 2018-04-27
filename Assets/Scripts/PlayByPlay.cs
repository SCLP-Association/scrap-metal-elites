using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayByPlay : MonoBehaviour {

	[Header("Play-by-play Announcer")]
	public float announcerVolume = 0.5f;
	public float idleTimeUntilRandom = 2.0f;
	public AudioSource audio;

	[Header("Voiceover Audio Clips")]
	public List<AudioClip> introClips;
	public List<AudioClip> onHitClips;
	public List<AudioClip> onDieClips;
	public List<AudioClip> randomClips;
	public List<AudioClip> outroClips;

	[Header("Entity Lists")]
	public Match matchManager; // this spawns the bots, so we ask it who they are
	private bool enumeratedBots = false; // delay init for a few frames while other things init
	private List<GameObject> bots; // list of bots we are commenting on
	private List<BotHealth> hps; // so we can look at health
	private List<AIController> ais; // so we can track ai state

	public void OnGameRecord(GameRecord record) {
		Debug.Log("PLAY-BY-PLAY: " + record.intValue + " = " + record.ToString());

		audio.PlayOneShot (onHitClips [Random.Range(0,onHitClips.Count)], announcerVolume);

		/*
		switch(record.intValue) { // this is just a counter FIXME
			case 0:
				audio.PlayOneShot (introClips [0], announcerVolume);
				break;
			case 65:
			audio.PlayOneShot (onHitClips [Random.Range(0,onHitClips.Count)], announcerVolume);
				break;
			case 100:
			audio.PlayOneShot (onHitClips [Random.Range(0,onHitClips.Count)], announcerVolume);
				break;
		}
		*/
	}



















	void Start () {
		// we don't look for bots here because
		// they may not have spawned yet
		//StartCoroutine(SoundOut());

		audio = GetComponent<AudioSource> ();
		audio.PlayOneShot (introClips [0], announcerVolume);

	}

	/*
	IEnumerator SoundOut()
	{
		while (keepPlaying){
			audio.PlayOneShot(introClips[0]);  
			yield return new WaitForSeconds(1.0f);
		}
	}
	*/

	public void someoneDied(GameObject who) { // TODO: determine "by whom"
		
		Debug.Log ("Play-by-play announcer: " + who.name + " BITES THE DUST!");

	}

	public void someoneTookDamage(int quantity) { // TODO: who?

		Debug.Log ("Play-by-play announcer: " + quantity + " DAMAGE!");

	}

	// Update is called once per frame
	void Update () {
	
		if (Time.time < 4.0f) { // wait for bots to be spawned
			return; // TODO: search each frame until they appear instead
		}

		if (!enumeratedBots && matchManager) { // needs to init?

			Debug.Log ("Play-by-play announcer: HERE COME THE COMPETITORS...");

			// enumerate them all
			bots = new List<GameObject>();
			hps = new List<BotHealth>();
			ais = new List<AIController>();

			// hardcoded for now
			bots.Add(matchManager.spawnedPlayer);
			bots.Add(matchManager.spawnedEnemy);

			// listen to events
			foreach(GameObject go in bots) {
				Debug.Log ("Play-by-play announcer: welcome [" + go.name + "]");
				BotHealth hp = go.GetComponentInChildren<BotHealth>();
				if (hp) {
					Debug.Log ("Play-by-play announcer: found a BotHealth!");
					hps.Add (hp);
					hp.onDeath.AddListener (someoneDied);
					hp.onChangePercent.AddListener (someoneTookDamage);
				}
				AIController ai = go.GetComponentInChildren<AIController>(true); // the true here means, "even inactive children" - but this still returns 0 hmmmmmm
				if (ai) {
					Debug.Log ("Play-by-play announcer: found an AIController!");
					ais.Add (ai);
				}
			}

			enumeratedBots = true;

			Debug.Log ("Play-by-play announcer: I found " + bots.Count + " competitors!");

		}

	}
}
