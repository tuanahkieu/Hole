using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource effectAudioSource;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioClip btnClick;
    [SerializeField] private AudioClip eat;
    [SerializeField] private AudioClip holeUp;
    [SerializeField] private AudioClip magnet;
    [SerializeField] private AudioClip freeze;
    [SerializeField] private AudioClip compass;
    [SerializeField] private AudioClip winMusic;
    [SerializeField] private AudioClip homeMusic;
    [SerializeField] private AudioClip loseMusic;

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
    public void playLoseMusic(){
        effectAudioSource.PlayOneShot(loseMusic);
    }
    public void playHomeMusic(){
        bgmAudioSource.clip = homeMusic;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }
    public void muteMusic(){
        bgmAudioSource.mute = !bgmAudioSource.mute;
    }
    public void muteEffect(){
        effectAudioSource.mute = !effectAudioSource.mute;
    }
    // public void stopHomeMusic(){
    //     if(effectAudioSource.clip == homeMusic) {
    //         effectAudioSource.Stop();
    //         effectAudioSource.loop = false;
    //     }
    // }

    
}
