using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverAudio : MonoBehaviour {

    public AudioClip themeSound;
    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        nextAudio();
    }

    void nextAudio()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(themeSound);
            audioSource.loop = true;
        }
    }
}
