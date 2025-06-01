using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using Random = System.Random;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioSource AudioSource2;
    public AudioSource AudioSource3;
    public AudioSource AudioSource4;
    public AudioSource AudioSource5;

    public AudioClip TileDiscard;// Сброс тайла
    public AudioClip TileHover;// Наведение мыши на тайл в руке
    public AudioClip CallSound;// Объявление пона кана чи
    public AudioClip CallTaking;// Появление тайлов в контейнере
    public AudioClip RonTsumo;
    public AudioClip HandFill;
    public AudioClip Music;
    public AudioClip ButtonClick;

    //public void Awake()
    //{
    //    AudioSource = GetComponent<AudioSource>();
    //}


    public void PlayTileDiscard()
    {
        AudioSource.PlayOneShot(TileDiscard);
    }

    public void PlayTileHover()
    {
        AudioSource2.PlayOneShot(TileHover);
    }

    public void PlayCallSound()
    {
        AudioSource.PlayOneShot(CallSound);
    }

    public void PlayCallTaking()
    {
        AudioSource3.PlayOneShot(CallTaking);
    }

    public void PlayRonTsumo()
    {
        AudioSource.PlayOneShot(RonTsumo);
    }

    public void PlayHandFill()
    {
        AudioSource4.PlayOneShot(HandFill);
    }

    public void PlayMusic()
    {
        AudioSource5.Stop();
        AudioSource5.PlayOneShot(Music);
    }

    public void StopMusic()
    {
        AudioSource5.Stop();
    }

    public void PlayButtonClick()
    {
        AudioSource3.PlayOneShot(ButtonClick);
    }
}
