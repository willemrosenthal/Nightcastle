using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

	public int numChannels = 6;
	public float sfxVolue = 1;
	public float bmgVolue = 1;

	public AudioMixer audioMixer;
	AudioMixerGroup[] audioMixGroup;

	[HideInInspector]public AudioSource bmg;
	List<AudioSource> channels;
	GameManager gm;

	List<AudioClip> musicQueue = new List<AudioClip>();


	void Awake () {
		gm = GameManager.Instance;
		//bmg = this.gameObject.AddComponent<AudioSource> ();
		//BuildChannels ();
		SetupAudioManager();
	}

	public void SetupAudioManager() {
		bmg = this.gameObject.AddComponent<AudioSource> ();
		BuildChannels ();
		SceneManager.sceneLoaded += NewSceneLoaded;
	}

	void BuildChannels() {
		audioMixGroup = audioMixer.FindMatchingGroups("Master");

		channels = new List<AudioSource> ();
		for (int i = 0; i < numChannels; i++) {
			channels.Add (this.gameObject.AddComponent<AudioSource> ());
			channels[i].playOnAwake = false;
			channels[i].outputAudioMixerGroup = audioMixGroup[0];
		}
	}

	public AudioSource GetChannel(int slot) {
		return channels[slot];
	}

	public void QueueMusic(List<AudioClip> tracks) {
		musicQueue = new List<AudioClip>();
		foreach (AudioClip t in tracks) {
			musicQueue.Add(t);
		}
		PlayQueuedMusic(true);
	}

	void PlayQueuedMusic(bool first = false) {
		if (bmg.isPlaying && !first) return;
		if (musicQueue.Count > 1) PlayBMG(musicQueue[0], false);
		if (musicQueue.Count == 1) PlayBMG(musicQueue[0], true);
		musicQueue.RemoveAt(0);
	}

	void Update() {
		if (musicQueue.Count > 0) {
			PlayQueuedMusic();
		}
	}

	public int Play(AudioClip clip, float volume = 1, float pitchVarriance = 0, float volVarriance = 0, float pitch = 1, float speed = 1, bool loop = false, int slot = -1, bool forceVol = false) {
		if (slot < 0 || slot >= channels.Count) {
			for (int i = 0; i < channels.Count; i++) {
				if (!channels [i].isPlaying) {
					slot = i;
					break;
				}
			}
			if (slot < 0 || slot >= channels.Count)
				slot = (int)Random.Range (0, channels.Count - 1);
		}
		channels [slot].volume = volume;
		channels [slot].loop = loop;
		channels [slot].clip = clip;
		channels [slot].pitch = pitch + (PlusOrMinus () * Random.value * pitchVarriance);

		// speed
		if (speed != 1) channels [slot].pitch = speed;
		//channels [slot].outputAudioMixerGroup.audioMixer.SetFloat("Pitch", 1f / speed);

		//channels [slot].pitch *= gm.gamePlaySpeed;
		channels [slot].volume = volume * (sfxVolue - (Random.value * volVarriance));
		if (forceVol) channels [slot].volume = volume;
		channels [slot].Play ();

		return slot;
	}

	

	public bool IsPlayingClip(AudioClip clip, int slot) {
		if (slot < 0 || slot >= channels.Count)
			return false;
		return (channels [slot].isPlaying && channels [slot].clip.name == clip.name);
	}

	public void Stop(int slot) {
		if (slot >= 0 || slot < channels.Count)
			channels [slot].Stop ();
	}

	public void Resume(int slot) {
		if (slot >= 0 || slot < channels.Count)
			channels [slot].Play ();
	}

	public void AlterSlot(int slot, float pitch = 1, float volume = 1) {
		if (slot >= 0 || slot < channels.Count) {
			channels [slot].pitch = pitch;
			channels [slot].volume = sfxVolue * volume;
		}
	}

	float PlusOrMinus() {
		return 1 - (Mathf.Round (Random.value) * 2);
	}

	public bool IsPlaying(int slot) {
		if (slot < 0 || slot >= channels.Count)
			return false;
		return channels [slot].isPlaying;
	}


	public void PlayBMG(AudioClip music, bool loop = true, float speed = 1, bool restart = true) {
//		Debug.Log (music);
		if (!restart && bmg.clip == music && bmg.isPlaying && bmg.pitch == speed) {
			bmg.volume = bmgVolue;
			return;
		}
		bmg.clip = music;
		bmg.loop = loop;
		bmg.volume = bmgVolue;
		bmg.pitch = speed;
		bmg.Play ();
	}

	public void SetBMGVolume(float vol) {
		bmgVolue = vol;
		bmg.volume = vol; 
	}

	public void PauseBMG() {
		bmg.Pause();
	}
	public void ResumeBMG() {
		if (!bmg.isPlaying) bmg.Play();
	}


	public void NewSceneLoaded(Scene scene, LoadSceneMode mode) {
		foreach (AudioSource c in channels) {
			if (c && c.isPlaying) c.Stop();
		}
	}
}
