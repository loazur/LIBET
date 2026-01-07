using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using System.Collections.Generic;


public class S_AudioManager : MonoBehaviour
{
    //! S_AudioManager contient tout les bus lié au sons du jeu, pour les réglés séparement

    public static S_AudioManager instance { get; private set; }

    //~ Les différents bus
    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance musicEventInstance;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            eventInstances = new List<EventInstance>();
            eventEmitters = new List<StudioEventEmitter>();

            masterBus = RuntimeManager.GetBus("bus:/");
            musicBus = RuntimeManager.GetBus("bus:/Music");
            sfxBus = RuntimeManager.GetBus("bus:/SFX");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InitializeMusic(S_FMODEvents.instance.music);
    }

    void OnDestroy()
    {
        CleanUp();
    }

    private void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        
        // stop all of the event emitters, because if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
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

    //! -------- Gestion de la musique -------

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    public void SetMusicArea(E_MusicArea area)
    {
        musicEventInstance.setParameterByName("area", (float)area);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    //! -------- Gestion des event emitters -------

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }


}
