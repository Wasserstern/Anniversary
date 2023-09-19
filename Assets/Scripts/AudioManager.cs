using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource musicSource;
    public AudioClip introSong;
    public AudioClip outroSong;
    public AudioClip loopSong;
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.clip = introSong;
    }

    void Update()
    {
        if(!musicSource.isPlaying && !musicSource.loop){
            musicSource.clip = loopSong;
            musicSource.Play();
            musicSource.loop = true;
        }
    }
}
