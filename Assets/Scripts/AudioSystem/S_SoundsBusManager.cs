using FMOD.Studio;
using UnityEngine;


public class S_SoundsBusManager : MonoBehaviour
{
    //! S_SoundsBugManager contient tout les bus lié au sons du jeu, pour les réglés séparement

    public static S_SoundsBusManager instance;

    //~ Les différents bus
    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
            musicBus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
            //sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        }
    }

    //!--------------------------------------------------

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

}
