using System;
using UnityEngine;
using UnityEngine.UI;

public class S_GameSettingsData : MonoBehaviour
{
    public static S_GameSettingsData instance;

    [Header("Elements d'UI")]
    [SerializeField] private Slider typingSpeedSlider;
    [SerializeField] private TMPro.TMP_Dropdown languageDropdown;

    public enum Languages
    {
        French,
        English
    }

    public enum CameraShake
    {
        Enabled,
        Lowered,
        Disabled
    }

    public enum ATHSizes
    {
        Big,
        Medium,
        Small
    }

    //! Valeurs par défauts
    private const Languages defaultLanguage = Languages.French;
    private const CameraShake defaultCameraShake = CameraShake.Enabled;
    private const ATHSizes defaultATHSize = ATHSizes.Medium;
    private const float defaultTypingSpeed = 7f;

    //! Actuellement utilisé
    public Languages currentLanguage { get; private set; } // Voir Enum
    public CameraShake currentCameraShake { get; private set; } // Voir Enum
    public ATHSizes currentATHSize { get; private set; } // Voir Enum
    public float currentTypingSpeed { get; private set; } // 1 - 50

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        LoadData();
    }

    //?------------------------------------- SETS

    public void setCurrentLanguage(int indexLanguage)
    {
        if ((int)currentLanguage == indexLanguage)
            return;

        currentLanguage = (Languages)indexLanguage;
        languageDropdown.value = (int)currentLanguage;

        OnLanguageChanged?.Invoke();
    }

    public void setCurrentCameraShake(int indexCameraShake)
    {
        if ((int)currentCameraShake == indexCameraShake)
            return;

        currentCameraShake = (CameraShake)indexCameraShake;

        OnCameraShakeChanged?.Invoke();
    }

    public void setCurrentATHSize(int indexATHSize)
    {
        if ((int)currentATHSize == indexATHSize)
            return;

        currentATHSize = (ATHSizes)indexATHSize;

        OnATHSizeChanged?.Invoke();
    }

    public void setCurrentTypingSpeed(float newTypingSpeed)
    {
        if (currentTypingSpeed == newTypingSpeed)
            return;

        currentTypingSpeed = newTypingSpeed;
        typingSpeedSlider.value = currentTypingSpeed;
        
    }

    //?------------------------------------- RESETS

    public void resetCurrentLanguage()
    {
        setCurrentLanguage((int)defaultLanguage);
    }

    public void resetCurrentCameraShake()
    {
        setCurrentCameraShake((int)defaultCameraShake);
    }

    public void resetCurrentATHSize()
    {
        setCurrentATHSize((int)defaultATHSize);
    }

    public void resetCurrentTypingSpeed()
    {
        setCurrentTypingSpeed(defaultTypingSpeed);
    }

    //?------------------------------------- EVENTS

    public event Action OnLanguageChanged;
    public event Action OnCameraShakeChanged;
    public event Action OnATHSizeChanged;


    //! ------------------------------------ SAVES/LOADS

    public void SaveData() //& Sauvegarde des données
    {
        // Met à jour les préferences
        PlayerPrefs.SetInt("GameLanguage", (int)currentLanguage);
        PlayerPrefs.SetInt("CameraShake", (int)currentCameraShake);
        PlayerPrefs.SetInt("ATHSize", (int)currentATHSize);
        PlayerPrefs.SetFloat("TypingSpeed", currentTypingSpeed);

        // Les sauvegarde dans PlayerPrefs
        PlayerPrefs.Save();
    }

    public void LoadData() //& Charge les données
    {
        //Charge les données si elles sont présentes sinon charge les valeurs par défaut
        if (PlayerPrefs.HasKey("GameLanguage"))  //~ Language
            setCurrentLanguage(PlayerPrefs.GetInt("GameLanguage"));
        else
            setCurrentLanguage((int)defaultLanguage);

        if (PlayerPrefs.HasKey("CameraShake")) //~ CameraShake
            setCurrentCameraShake(PlayerPrefs.GetInt("CameraShake"));
        else
            setCurrentLanguage((int)defaultCameraShake);

        if (PlayerPrefs.HasKey("ATHSize")) //~ ATHSize
            setCurrentATHSize(PlayerPrefs.GetInt("ATHSize"));
        else
            setCurrentATHSize((int)defaultATHSize);

        if (PlayerPrefs.HasKey("TypingSpeed")) //~ TypingSpeed
            setCurrentTypingSpeed(PlayerPrefs.GetFloat("TypingSpeed"));
        else
            setCurrentTypingSpeed(defaultTypingSpeed);
    }

    
}
