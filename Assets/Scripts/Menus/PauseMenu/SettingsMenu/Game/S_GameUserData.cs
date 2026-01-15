using System;
using UnityEngine;
using UnityEngine.UI;

public class S_GameUserData : MonoBehaviour
{
    public static S_GameUserData instance;

    [Header("Gestion de l'UI")]
    [SerializeField] private TMPro.TMP_Dropdown dropdownLanguage; //! Dropdown du langage
    [SerializeField] private Slider sliderTypingSpeed; //! Slider de la vitesse d'écriture
    [SerializeField] private Toggle cameraShakeToggle; //! Toggle du temblement de la camera

    public enum Languages
    {
        French,
        English
    }

    public enum ATHSizes
    {
        Big,
        Medium,
        Small
    }

    //! Valeurs par défauts
    private const Languages defaultLanguage = Languages.French;
    private const bool defaultCameraShake = true;
    private const ATHSizes defaultATHSize = ATHSizes.Medium;
    private const float defaultTypingSpeed = 7f;

    //! Actuellement utilisé
    public Languages currentLanguage { get; private set; } // Voir Enum
    public bool currentCameraShake { get; private set; } // Voir Enum
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
        dropdownLanguage.value = (int)currentLanguage;

        OnLanguageChanged?.Invoke();
    }

    public void setCurrentCameraShake(bool enabled)
    {
        if (currentCameraShake == enabled)
            return;

        currentCameraShake = enabled;
        cameraShakeToggle.isOn = currentCameraShake;
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
        sliderTypingSpeed.value = currentTypingSpeed;
        
    }

    //?------------------------------------- RESETS

    public void resetCurrentLanguage()
    {
        setCurrentLanguage((int)defaultLanguage);
    }

    public void resetCurrentCameraShake()
    {
        setCurrentCameraShake(defaultCameraShake);
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
    public event Action OnATHSizeChanged;


    //! ------------------------------------ SAVES/LOADS

    public void SaveData() //& Sauvegarde des données
    {
        // Met à jour les préferences
        PlayerPrefs.SetInt("GameLanguage", (int)currentLanguage);
        PlayerPrefs.SetInt("CameraShake", Convert.ToInt32(currentCameraShake));
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
            resetCurrentLanguage();

        if (PlayerPrefs.HasKey("CameraShake")) //~ CameraShake
            setCurrentCameraShake(Convert.ToBoolean(PlayerPrefs.GetInt("CameraShake")));
        else
            resetCurrentCameraShake();

        if (PlayerPrefs.HasKey("ATHSize")) //~ ATHSize
            setCurrentATHSize(PlayerPrefs.GetInt("ATHSize"));
        else
            resetCurrentATHSize();

        if (PlayerPrefs.HasKey("TypingSpeed")) //~ TypingSpeed
            setCurrentTypingSpeed(PlayerPrefs.GetFloat("TypingSpeed"));
        else
            resetCurrentTypingSpeed();
    }

    
}
