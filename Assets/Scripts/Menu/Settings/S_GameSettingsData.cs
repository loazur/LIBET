using System;
using System.Runtime.CompilerServices;
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
    private Languages defaultLanguage = Languages.French;
    private CameraShake defaultCameraShake = CameraShake.Enabled;
    private ATHSizes defaultATHSize = ATHSizes.Medium;
    private float defaultTypingSpeed = 7f;

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

        SetupGameSettings();
    }

    private void SetupGameSettings()
    {
        currentLanguage = defaultLanguage;
        currentCameraShake = defaultCameraShake;
        currentATHSize = defaultATHSize;
        currentTypingSpeed = defaultTypingSpeed;
    }

    //?------------------------------------- SETS

    public void setCurrentLanguage(int indexLanguage)
    {
        if ((int)currentLanguage == indexLanguage)
            return;

        currentLanguage = (Languages)indexLanguage;

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
    }

    //?------------------------------------- RESETS

    public void resetCurrentLanguage()
    {
        setCurrentLanguage((int)defaultLanguage);
        languageDropdown.value = (int)currentLanguage;

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
        typingSpeedSlider.value = currentTypingSpeed;
    }

    //?------------------------------------- EVENTS

    public event Action OnLanguageChanged;
    public event Action OnCameraShakeChanged;
    public event Action OnATHSizeChanged;
    



}
