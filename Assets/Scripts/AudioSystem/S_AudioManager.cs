using FMOD.Studio;
using UnityEngine;
using FMODUnity;


public class S_AudioManager : MonoBehaviour
{
    //! S_AudioManager contient tout les bus lié au sons du jeu, pour les réglés séparement

    public static S_AudioManager instance { get; private set; }

    //~ Les différents bus
    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    private EventInstance currentMusicInstance;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            masterBus = RuntimeManager.GetBus("bus:/");
            musicBus = RuntimeManager.GetBus("bus:/Music");
            sfxBus = RuntimeManager.GetBus("bus:/SFX");
        }
    }

    //!---------Gestion des volumes des bus---------

    public void SetMasterVolume(float volume)
    {
        masterBus.setVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicBus.setVolume(volume);
    }

    public void SetSoundEffectsVolume(float volume)
    {
        sfxBus.setVolume(volume);
    }


    //? Récupérer les volumes

    public float GetMasterVolume()
    {
        masterBus.getVolume(out float volume);
        return volume;
    }

    public float GetMusicVolume()
    {
        musicBus.getVolume(out float volume);
        return volume;
    }
    
    public float GetSoundEffectsVolume()
    {
        sfxBus.getVolume(out float volume);
        return volume;
    }

    

    //! -------- Jouer des sons -------

    public void PlayOneShot(EventReference sound, Vector3 worldPos) //& Joue un son une fois
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    //! Gestion de la musique 

    public void PlayMusic(EventReference music, Vector3 position)
    {
        StopMusic();

        currentMusicInstance = RuntimeManager.CreateInstance(music);
        currentMusicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        currentMusicInstance.start();
    }

    public void StopMusic()
    {
        if (currentMusicInstance.isValid())
        {
            currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentMusicInstance.release();
        }
    }


}
