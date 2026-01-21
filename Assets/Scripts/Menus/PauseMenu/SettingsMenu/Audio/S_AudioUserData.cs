using UnityEngine;
using UnityEngine.UI;

public class S_AudioUserData : MonoBehaviour
{
    public static S_AudioUserData instance;

    [Header("Gestion de l'UI")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider soundEffectsVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    //! Valeurs par défauts
    private const float defaultMasterVolume = 1f;
    private const float defaultSoundEffectsVolume = 0.3f;
    private const float defaultMusicVolume = 0.3f;

    //! Actuellement utilisé
    //! Voir S_SoundsBusManager
    
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
        S_AudioManager.instance.SetMasterVolume(volume);

        masterVolumeSlider.value = volume;
    }

    public void setCurrentSoundEffectsVolume(float volume)
    {
        S_AudioManager.instance.SetSoundEffectsVolume(volume);

        soundEffectsVolumeSlider.value = volume;
    }

    public void setCurrentMusicVolume(float volume)
    {
        S_AudioManager.instance.SetMusicVolume(volume);

        musicVolumeSlider.value = volume;
    }

    //?------------------------------------- RESETS

    public void resetMasterVolume()
    {   
        setCurrentMasterVolume(defaultMasterVolume);
    }

    public void resetSoundEffectsVolume()
    {
        setCurrentSoundEffectsVolume(defaultSoundEffectsVolume);
    }

    public void resetMusicVolume()
    {
        setCurrentMusicVolume(defaultMusicVolume);
    }

    //?------------------------------------- EVENTS


    //! ------------------------------------ SAVES/LOADS

    public void SaveData() //& Sauvegarde des données
    {
        PlayerPrefs.SetFloat("MasterVolume", S_AudioManager.instance.GetMasterVolume());
        PlayerPrefs.SetFloat("SFXVolume", S_AudioManager.instance.GetSoundEffectsVolume());
        PlayerPrefs.SetFloat("MusicVolume", S_AudioManager.instance.GetMusicVolume());

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

        if (PlayerPrefs.HasKey("SFXVolume")) //~ CameraShake
            setCurrentSoundEffectsVolume(PlayerPrefs.GetFloat("SFXVolume"));
        else
            resetSoundEffectsVolume();

        if (PlayerPrefs.HasKey("MusicVolume")) //~ ATHSize
            setCurrentMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        else
            resetMusicVolume();
    }

    
}
