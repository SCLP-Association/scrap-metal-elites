using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayByPlay : MonoBehaviour {

	[Header("Play-by-play Announcer")]
	public UnitFloatVariable announcerVolume;
	public UnitFloatVariable crowdVolume;
	public float idleTimeUntilRandom = 2.0f;

	[Header("Voiceover Audio Clips")]
	public List<AudioClip> crowdClips;
	public List<AudioClip> introClips;
	public List<AudioClip> onHitClips;
	public List<AudioClip> onDieClips;
	public List<AudioClip> randomClips;
	public List<AudioClip> outroClips;

    private AudioSource myAudioSource;
    private int quipCounter = 0; // so we don't repeat outselves 
    private float timeSinceLastSpoke = 0f; // to find awkward silence
    private float awkwardSilenceTimespan = 2f; // seconds of silence before a random quip is uttered
    private AudioClip pendingClip = null; // wait for previous if we would interrupt ourselves

    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    // callback for gameplay events
    public void OnGameRecord(GameRecord record) {

        AudioClip playMe = null;

        Debug.Log("PLAY-BY-PLAY: " + record.ToString());

        switch (record.tag) // which kind of event was it?
        {
            case GameRecordTag.BotDied:
                playMe = onDieClips[quipCounter % onDieClips.Count];
                break;
            case GameRecordTag.BotFlipped:
                playMe = onHitClips[quipCounter % onHitClips.Count];
                break;
            case GameRecordTag.BotJointBroke:
                playMe = onHitClips[quipCounter % onHitClips.Count];
                break;
            case GameRecordTag.GameEnemyDeclared:
                playMe = introClips[quipCounter % introClips.Count];
                break;
            case GameRecordTag.GameFinished:
                playMe = outroClips[quipCounter % outroClips.Count];
                break;
            case GameRecordTag.GamePlayerDeclared:
                playMe = introClips[quipCounter % introClips.Count];
                break;
            case GameRecordTag.GamePrepared:
                playMe = introClips[quipCounter % introClips.Count];
                break;
            case GameRecordTag.GameStarted:
                playMe = crowdClips[quipCounter % crowdClips.Count];
                break;
            case GameRecordTag.BotTookDamage:
                playMe = onHitClips[quipCounter % onHitClips.Count];
                break;
        }

        if (!myAudioSource.isPlaying && playMe) { // don't interrupt yourself
			myAudioSource.PlayOneShot (playMe, announcerVolume.Value);
		} else {
			Debug.Log ("PLAY-BY-PLAY: overlapping voiceover queued...");
            pendingClip = playMe;

        }

        quipCounter++; // so we don't get repeats

    }

    void Update () {
	
		// user quality of life feature: ESC will insta-shutup any announcer at all times
		if (Input.GetKey (KeyCode.Escape)) {
			myAudioSource.Stop ();
		}

        // possibly play a pending voiceover clip
        if (pendingClip && !myAudioSource.isPlaying) {
            Debug.Log("PLAY-BY-PLAY: playing previously queued clip");
            myAudioSource.PlayOneShot(pendingClip, announcerVolume.Value);
            pendingClip = null;
        }

        // if we are in the middle of an akward silence, say something
        if (myAudioSource.isPlaying) {
			timeSinceLastSpoke = Time.time;
		} else {
			if ((Time.time - timeSinceLastSpoke > awkwardSilenceTimespan) && (quipCounter > 2)) {
				Debug.Log ("PLAY-BY-PLAY: filling awkward silence with colour commentary.");
				myAudioSource.PlayOneShot (randomClips [quipCounter % randomClips.Count], announcerVolume.Value);
			} // time
		} // silence

	}
}
