using UnityEngine;

public class MenuAudioPlayer:MonoBehaviour
{
    public AudioSource AudioSource1;
    public AudioSource AudioSource2;

    public AudioClip Music;
    public AudioClip ButtonClick;

    public void PlayMusic()
    {
        AudioSource1.PlayOneShot(Music);
        
    }

    public void StopMusic()
    {
        AudioSource1.Stop();
    }

    public void PlayButtonClick()
    {
        AudioSource2.PlayOneShot(ButtonClick);
    }
}
