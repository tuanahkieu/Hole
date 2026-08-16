using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource effectAudioSource;
    [SerializeField] private AudioClip btnClick;
    [SerializeField] private AudioClip eat;
    [SerializeField] private AudioClip holeUp;
    [SerializeField] private AudioClip magnet;
    [SerializeField] private AudioClip freeze;
    [SerializeField] private AudioClip compass;
    [SerializeField] private AudioClip winMusic;
    [SerializeField] private AudioClip homeMusic;

    public void playBtnClickSound(){
        effectAudioSource.PlayOneShot(btnClick);
    }

    public void playEatSound(){
        effectAudioSource.PlayOneShot(eat);
    }

    public void playHoleUpSound(){
        effectAudioSource.PlayOneShot(holeUp);
    }

    public void playMagnetSound(){
        effectAudioSource.clip = magnet;
        effectAudioSource.loop = true;
        effectAudioSource.Play();
    }

    public void stopMagnetSound(){
        if(effectAudioSource.clip == magnet) {
            effectAudioSource.Stop();
            effectAudioSource.loop = false;
        }
    }
    public void playFreezeSound(){
        effectAudioSource.PlayOneShot(freeze);
    }
    public void stopFreezeSound(){
        if(effectAudioSource.clip == freeze) {
            effectAudioSource.Stop();
            effectAudioSource.loop = false;
        }
    }
    public void playCompassSound(){
        effectAudioSource.PlayOneShot(compass);
    }
    public void playWinMusic(){
        effectAudioSource.PlayOneShot(winMusic);
    }
    public void playHomeMusic(){
        effectAudioSource.clip = homeMusic;
        effectAudioSource.loop = true;
        effectAudioSource.Play();
    }
    // public void stopHomeMusic(){
    //     if(effectAudioSource.clip == homeMusic) {
    //         effectAudioSource.Stop();
    //         effectAudioSource.loop = false;
    //     }
    // }

    
}
