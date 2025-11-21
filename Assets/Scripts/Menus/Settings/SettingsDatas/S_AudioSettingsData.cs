using System;
using UnityEngine;
using UnityEngine.UI;

public class S_AudioSettingsData : MonoBehaviour
{
    public static S_AudioSettingsData instance;

    [Header("Gestion de l'UI")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider soundFXVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;


    //! Valeurs par défauts
    private const float  defaultMasterVolume = 1f;
    private const float  defaultSoundFXVolume = 1f;
    private const float  defaultMusicVolume = 1f;

    //! Actuellement utilisé
    /*
    public float currentMasterVolume {get; private set;}
    public float currentSoundFXVolume {get; private set;}
    public float currentMusicVolume {get; private set;}
    */

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        LoadData();
    }

    //?------------------------------------- SETS

    //! Dans S_SoundMixerManager

    //?------------------------------------- RESETS

    public void resetMasterVolume()
    {
        S_SoundMixerManager.instance.SetMasterVolume(defaultMasterVolume);
    }

    public void resetSoundFXVolume()
    {
        S_SoundMixerManager.instance.SetSoundFXVolume(defaultSoundFXVolume);
    }

    public void resetMusicVolume()
    {
        S_SoundMixerManager.instance.SetMusicVolume(defaultMusicVolume);
    }

   

    //?------------------------------------- EVENTS


    //! ------------------------------------ SAVES/LOADS

    public void SaveData() //& Sauvegarde des données
    {
        // Met à jour les préferences*
        float masterVolume;
        S_SoundMixerManager.instance.audioMixer.GetFloat("masterVolume", out masterVolume);

        float soundFXVolume;
        S_SoundMixerManager.instance.audioMixer.GetFloat("soundFXVolume", out soundFXVolume);

        float musicVolume;
        S_SoundMixerManager.instance.audioMixer.GetFloat("musicVolume", out musicVolume);

        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("SoundFXVolume", soundFXVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        // Les sauvegarde dans PlayerPrefs
        PlayerPrefs.Save();
    }

    public void LoadData() //& Charge les données
    {
        //Charge les données si elles sont présentes sinon charge les valeurs par défaut
        if (PlayerPrefs.HasKey("MasterVolume"))  //~ Language
            S_SoundMixerManager.instance.SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        else
            resetMasterVolume();

        if (PlayerPrefs.HasKey("SoundFXVolume")) //~ CameraShake
            S_SoundMixerManager.instance.SetMasterVolume(PlayerPrefs.GetFloat("SoundFXVolume"));
        else
            resetSoundFXVolume();

        if (PlayerPrefs.HasKey("MusicVolume")) //~ ATHSize
            S_SoundMixerManager.instance.SetMasterVolume(PlayerPrefs.GetFloat("MusicVolume"));
        else
            resetMusicVolume();
    }

    
}
