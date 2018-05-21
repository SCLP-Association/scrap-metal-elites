using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayByPlay : MonoBehaviour {

/*
	[Header("Arena Soundsystem")]
	public float musicVolume = 0.25f;
	public List<AudioClip> musicTracks;
	public AudioSource music;
*/

	[Header("Play-by-play Announcer")]
	//public float announcerVolume = 0.5f;
	//public float crowdVolume = 0.5f;
	public UnitFloatVariable announcerVolume;
	public UnitFloatVariable crowdVolume;
	public float idleTimeUntilRandom = 2.0f;
	public AudioSource audio;

	private int prevClipNum = -1; // avoid duplicate exclamations
	private float timeSinceLastSpoke = 0f; // to find awkward silence
	private float awkwardSilenceTimespan = 2f; // seconds of silence before a random quip is uttered
	private int prevRandomClipNum = -1; // to avoid dupes


	[Header("Voiceover Audio Clips")]
	public List<AudioClip> crowdClips;
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

		Debug.Log("PLAY-BY-PLAY: event #" + record.intValue + ": " + record.GetType() + " decription= " + record.ToString());
		// is there no way to find out WHICH kind of event was passed?

/*
		if (music) {
			if (record.intValue==2) { // second event of the match: time for music
				Debug.Log("PLAY-BY-PLAY: playing music..."); // fixme: when done, play another track
				music.PlayOneShot(musicTracks [Random.Range (0, musicTracks.Count)], musicVolume);
			}
		}
*/


		if (!audio.isPlaying) { // don't interrupt yourself
			int nextClipNum = -1;
			while (nextClipNum == -1 || prevClipNum == nextClipNum) { // choose a new clip, ignore the prev
				nextClipNum = Random.Range (0, onHitClips.Count);
			}
			audio.PlayOneShot (onHitClips [nextClipNum], announcerVolume.Value);
			prevClipNum = nextClipNum;
		} else {
			Debug.Log ("PLAY-BY-PLAY: overlapping event ignored so I don't interrupt myself... TODO: queue up?");
		}

		// crowd reaction!
		// only 25% of the time they applaud
		// FIXME: react approriately depending on how good it is?
		if (record.intValue > 2) { 		// ignore intro
			// first hit always gets a reaction! (the other two are the init events, usually...)
			if (record.intValue == 3)
				audio.PlayOneShot (crowdClips [Random.Range (0, crowdClips.Count)], crowdVolume.Value);
			if (Random.Range (0, 4) == 1)
				audio.PlayOneShot (crowdClips [Random.Range (0, crowdClips.Count)], crowdVolume.Value);
		}

		/*
		switch(record.intValue) { // this is just a counter FIXME
			case 0:
				audio.PlayOneShot (introClips [0], announcerVolume.Value);
				break;
			case 65:
			audio.PlayOneShot (onHitClips [Random.Range(0,onHitClips.Count)], announcerVolume.Value);
				break;
			case 100:
			audio.PlayOneShot (onHitClips [Random.Range(0,onHitClips.Count)], announcerVolume.Value);
				break;
		}
		*/
	}



















	void Start () {
		// we don't look for bots here because
		// they may not have spawned yet
		//StartCoroutine(SoundOut());

		audio = GetComponent<AudioSource> ();
		audio.PlayOneShot (introClips [0], announcerVolume.Value); // FIXME wait for game start event

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
	
		// user quality of life feature: ESC will insta-shutup any announcer at all times
		if (Input.GetKey (KeyCode.Escape)) {
			//Debug.Log ("Play-by-play announcer: ESC pressed: shutting up.");	// spammy: not debounced
			audio.Stop ();
		}

		// if we are in the middle of an akward silence, say something
		if (audio.isPlaying) {
			timeSinceLastSpoke = Time.time;
		} else {
			if (Time.time - timeSinceLastSpoke > awkwardSilenceTimespan) {
				Debug.Log ("PLAY-BY-PLAY: filling awkward silence with colour commentary.");
				int nextRandomClipNum = -1;
				while (nextRandomClipNum == -1 || prevRandomClipNum == nextRandomClipNum) { // choose a new clip, ignore the prev
					nextRandomClipNum = Random.Range (0, randomClips.Count);
				}
				audio.PlayOneShot (randomClips [nextRandomClipNum], announcerVolume.Value);
				prevRandomClipNum = nextRandomClipNum;
			} // time
		} // silence


		/*
		 * // the old idea - scrap this? how to enumerate bots?

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
		*/

	}
}
