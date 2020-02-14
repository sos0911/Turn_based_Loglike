using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

	public AudioSource efxSource;
	public AudioSource musicSource;
	public static SoundManager instance = null; // singleton

	public float lowPitchRange = .95f;
	public float highPitchRange = 1.05f;

	private float volval = 1f; // 처음엔 볼륨 max

	// Use this for initialization
	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
		// scene이 넘어가도 이 사운드매니저는 제거되지 않음
		DontDestroyOnLoad(gameObject);
	}

	public void PlaySingle(AudioClip clip)
	{
		efxSource.clip = clip;
		efxSource.Play();
	}
	
	// params : 쉼표로 구분되는 한 몇개라도 인풋으로 가져올 수 있음
	// 몇개를 입력하든 간에 한 배열로 만들어져 인풋으로 넘겨진다.
	// audiosource vs audioclip ?? 
	public void RandomizeSfx(params AudioClip[] clips)
	{
		int randomindex = Random.Range(0, clips.Length);
		float randompitch = Random.Range(lowPitchRange, highPitchRange);

		efxSource.pitch = randompitch;
		efxSource.clip = clips[randomindex];
		efxSource.Play();	
	}
	
}
