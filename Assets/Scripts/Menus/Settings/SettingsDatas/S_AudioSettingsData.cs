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
    //! Voir S_SoundMixerManager
    

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        LoadData();
    }

    //?------------------------------------- SETS

    public void setCurrentMasterVolume(float volume)
    {
        S_SoundMixerManager.instance.SetMasterVolume(volume);

        masterVolumeSlider.value = volume;
    }

    public void setCurrentSoundFXVolume(float volume)
    {
        S_SoundMixerManager.instance.SetSoundFXVolume(volume);

        soundFXVolumeSlider.value = volume;
    }

    public void setCurrentMusicVolume(float volume)
    {
        S_SoundMixerManager.instance.SetMusicVolume(volume);

        musicVolumeSlider.value = volume;
    }

    //?------------------------------------- RESETS

    public void resetMasterVolume()
    {   
        setCurrentMasterVolume(defaultMasterVolume);
    }

    public void resetSoundFXVolume()
    {
        setCurrentSoundFXVolume(defaultSoundFXVolume);
    }

    public void resetMusicVolume()
    {
        setCurrentMusicVolume(defaultMusicVolume);
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

        PlayerPrefs.SetFloat("MasterVolume", Mathf.Pow(10f, masterVolume / 20f));
        PlayerPrefs.SetFloat("SoundFXVolume", Mathf.Pow(10f, soundFXVolume / 20f));
        PlayerPrefs.SetFloat("MusicVolume", Mathf.Pow(10f, musicVolume / 20f));

        // Les sauvegarde dans PlayerPrefs
        PlayerPrefs.Save();
    }

    public void LoadData() //& Charge les données
    {
        //Charge les données si elles sont présentes sinon charge les valeurs par défaut
        if (PlayerPrefs.HasKey("MasterVolume"))  //~ Language
            setCurrentMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        else
            resetMasterVolume();

        if (PlayerPrefs.HasKey("SoundFXVolume")) //~ CameraShake
            setCurrentSoundFXVolume(PlayerPrefs.GetFloat("SoundFXVolume"));
        else
            resetSoundFXVolume();

        if (PlayerPrefs.HasKey("MusicVolume")) //~ ATHSize
            setCurrentMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        else
            resetMusicVolume();
    }

    
}
