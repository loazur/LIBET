using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class S_VideoSettingsData : MonoBehaviour
{
    public static S_VideoSettingsData instance;

    [Header("Gestion de l'UI ")]
    [SerializeField] private TMPro.TMP_Dropdown dropdownResolution; //! Dropdown de la resolution
    [SerializeField] private TMPro.TMP_Dropdown dropdownParticlesEffects; //! Dropdown des effets de particules
    [SerializeField] private Slider sliderFPSMax; //! Slider des FPS max
    [SerializeField] private Toggle toggleFullScreen; //! Toggle du plein écran
    [SerializeField] private Toggle toggleVSync; //! Toggle du VSync

    Resolution[] availableResolutions; // Résolutions disponibles

    public enum ParticlesEffects
    {
        Enabled,
        Lowered,
        Disabled
    }
    
    //! Valeurs par défauts
    private const int defaultResolutionIndex = 0; // Max Résolution
    private const int defaultParticlesEffectsIndex = (int)ParticlesEffects.Enabled; // Particules activés
    private const int defaultFPSMax = 144; // 144 fps max
    private const bool defaultFullScreen = true; // Plein écran
    private const bool defaultVSync = false; // Désactivé

    //! Actuellement utilisé
    public int currentResolutionIndex {get; private set;}
    public int currentParticlesEffectsIndex {get; private set;}
    public int currentFPSMax {get; private set;}
    public bool currentFullScreen {get; private set;}
    public bool currentVSync {get; private set;}


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        SetupResolutions();
        SetupParticlesEffects();

        LoadData();
    }

    //?------------------------------------- SETS

    public void setCurrentResolution(int indexResolution)
    {
        if (currentResolutionIndex == indexResolution)
            return;

        Resolution newResolution = availableResolutions[indexResolution];

        currentResolutionIndex = indexResolution;
        Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreen);

        dropdownResolution.value = indexResolution;
    }

    public void setCurrentFullScreen(bool enabled)
    {
        if (currentFullScreen == enabled)
            return;

        Screen.fullScreen = enabled;
        currentFullScreen = Screen.fullScreen;

        toggleFullScreen.isOn = currentFullScreen;
    }

    public void setCurrentParticlesEffects(int newParticleEffects)
    {
        if (currentParticlesEffectsIndex == newParticleEffects)
            return;

        currentParticlesEffectsIndex = newParticleEffects;
        dropdownParticlesEffects.value = currentParticlesEffectsIndex;
    }

    public void setCurrentFPSMax(float newFPSMax)
    {
        if (currentFPSMax == newFPSMax)
            return;

    
        if (newFPSMax == sliderFPSMax.maxValue)
            Application.targetFrameRate = -1; // Illimité
        else
            Application.targetFrameRate = (int)newFPSMax; // Changement de la limite

        currentFPSMax = (int)newFPSMax;
        sliderFPSMax.value = currentFPSMax;
    }

    public void setCurrentVSync(bool enabled)
    {
        if (currentVSync == enabled)
            return;

        QualitySettings.vSyncCount = Convert.ToInt32(enabled);
        currentVSync = enabled;
        toggleVSync.isOn = currentVSync;
    }

    //?------------------------------------- RESETS

    public void resetCurrentResolution()
    {
        setCurrentResolution(defaultResolutionIndex);
    }

    public void resetFullScreen()
    {
        setCurrentFullScreen(defaultFullScreen);
    }

    public void resetParticlesEffects()
    {
        setCurrentParticlesEffects(defaultParticlesEffectsIndex);
    }
    
    public void resetCurrentFPSMax()
    {
        setCurrentFPSMax(defaultFPSMax);
    }

    public void resetCurrentVSync()
    {
        setCurrentVSync(defaultVSync);
    }

    //?------------------------------------- EVENTS


    //! ------------------------------------ SAVES/LOADS

    public void SaveData() //& Sauvegarde des données
    {
        // Met à jour les préferences
        PlayerPrefs.SetInt("Resolution", currentResolutionIndex);
        PlayerPrefs.SetInt("ParticlesEffects", currentParticlesEffectsIndex);
        PlayerPrefs.SetInt("FPSMax", currentFPSMax);
        PlayerPrefs.SetInt("FullScreen", Convert.ToInt32(currentFullScreen));
        PlayerPrefs.SetInt("VSync", Convert.ToInt32(currentVSync));

        // Les sauvegarde dans PlayerPrefs
        PlayerPrefs.Save();
    }

    public void LoadData() //& Charge les données
    {
        //Charge les données si elles sont présentes sinon charge les valeurs par défaut
        if (PlayerPrefs.HasKey("FullScreen")) //~ FullScreen
            setCurrentFullScreen(Convert.ToBoolean(PlayerPrefs.GetInt("FullScreen")));
        else
            setCurrentFullScreen(defaultFullScreen);

        if (PlayerPrefs.HasKey("Resolution")) //~ Resolution
            setCurrentResolution(PlayerPrefs.GetInt("Resolution"));
        else
            setCurrentResolution(defaultResolutionIndex);

        if (PlayerPrefs.HasKey("ParticlesEffects")) //~ ParticlesEffects
            setCurrentParticlesEffects(PlayerPrefs.GetInt("ParticlesEffects"));
        else
            setCurrentParticlesEffects(defaultParticlesEffectsIndex);

        if (PlayerPrefs.HasKey("FPSMax")) //~ FPSMax
            setCurrentFPSMax(PlayerPrefs.GetInt("FPSMax"));
        else
            setCurrentFPSMax(defaultFPSMax);

        if (PlayerPrefs.HasKey("VSync")) //~ VSync
            setCurrentVSync(Convert.ToBoolean(PlayerPrefs.GetInt("VSync")));
        else
            setCurrentVSync(defaultVSync);
    }

    private void SetupResolutions() //& Charge les résolutions
    {
        // Récupère toutes les resolutions possible et les inverse dans la liste (plus grand = index 0)
        availableResolutions = Screen.resolutions;
        Array.Reverse(availableResolutions); 

        List<string> resolutionToString = new List<string>(); // Liste pour un affichage plus simple
        string resolutionText; // Texte qui va etre affiché dans la liste déroulante

        foreach(Resolution resolution in availableResolutions)
        {
            resolutionText = resolution.width + " x " + resolution.height; // Change l'affichage des résolution

            if (!resolutionToString.Contains(resolutionText))
            {
                resolutionToString.Add(resolutionText);
            }
        }

        dropdownResolution.AddOptions(resolutionToString); // Ajoute les résolution au dropdown
    }

    private void SetupParticlesEffects() //& Charge les différent type d'effets de particules
    {
        //! Probleme ducoup je choisi pas le texte sa se base sur l'enum
        dropdownParticlesEffects.AddOptions(Enum.GetNames(typeof(ParticlesEffects)).ToList());
    }
}
