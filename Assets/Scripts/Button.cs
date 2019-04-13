using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    public AudioClip hightlightSound;
    public AudioClip pressedSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayHightlight()
    {
        audioSource.PlayOneShot(hightlightSound);
    }

    public void PlayPressed()
    {
        audioSource.PlayOneShot(pressedSound);
    }
}
