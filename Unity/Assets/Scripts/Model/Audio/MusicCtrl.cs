using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicCtrl : MonoBehaviour
{
    public static MusicCtrl instance;
    private AudioListener audioListener;
    private AudioSource audioSource;
    [HideInInspector]
    public bool audioSwitch;
 
    void Start()
    {
        instance = this;
        audioSwitch = PlayerPrefs.GetInt("audioSwitch") > 0;
        audioListener = this.GetComponent<AudioListener>();
        audioSource = this.GetComponent<AudioSource>();
    }

    public void RefreshAudioSwitch()
    {
        audioSwitch = !audioSwitch;
        audioListener.enabled = audioSwitch;
        PlayerPrefs.SetInt("audioSwitch", audioSwitch ? 1 : 0);
    }

    public void Play(string name)
    {

        //audioSource.PlayOneShot();
    }
}
