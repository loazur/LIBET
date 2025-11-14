using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_VideoSettingsData : MonoBehaviour
{
    public static S_VideoSettingsData instance;

    [Header("Gestion de l'UI ")]
    [SerializeField] private TMPro.TMP_Dropdown dropdownResolution; //! Dropdown de la resolution
    [SerializeField] private TMPro.TMP_Dropdown dropdownFullscreen;  //! Dropdown de fullscreen
    [SerializeField] private TMPro.TMP_Dropdown dropdownParticlesEffects; //! Dropdown des effets de particules
    [SerializeField] private Slider sliderFPSMax; //! Slider des FPS max
    [SerializeField] private Toggle toggleVSync; //! Toggle du VSync

    Resolution[] availableResolutions;
    
    //! Valeurs par défauts
    //TODO
    private const float defaultFPSMax = 144f;
    private const bool defaultVSync = false;

    //! Actuellement utilisé
    //TODO
    
    //? currentResolution = Screen.currentResolution
    public float currentFPSMax {get; private set;}
    public bool currentVSync {get; private set;}


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        Debug.Log(Screen.currentResolution);

        SetupResolutions();
        LoadData();
    }

    //?------------------------------------- SETS

    //TODO

    public void setCurrentFPSMax(float newFPSMax)
    {
        if (currentFPSMax == newFPSMax)
            return;

        currentFPSMax = newFPSMax;
        sliderFPSMax.value = currentFPSMax;
    }

    public void setCurrentVSync(bool enabled)
    {
        if (currentVSync == enabled)
            return;

        currentVSync = enabled;
        toggleVSync.isOn = currentVSync;
    }

    //?------------------------------------- RESETS

    //TODO

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
        //TODO
        
        PlayerPrefs.SetFloat("FPSMax", currentFPSMax);
        PlayerPrefs.SetInt("VSync", Convert.ToInt32(currentVSync));

        // Les sauvegarde dans PlayerPrefs
        PlayerPrefs.Save();
    }

    public void LoadData() //& Charge les données
    {
        //Charge les données si elles sont présentes sinon charge les valeurs par défaut
        //TODO   

        if (PlayerPrefs.HasKey("FPSMax")) //~ FPSMax
            setCurrentFPSMax(PlayerPrefs.GetFloat("FPSMax"));
        else
            setCurrentFPSMax(defaultFPSMax);

        if (PlayerPrefs.HasKey("VSync")) //~ VSync
            setCurrentVSync(Convert.ToBoolean(PlayerPrefs.GetInt("VSync")));
        else
            setCurrentVSync(defaultVSync);
    }

    private void SetupResolutions()
    {
        availableResolutions = Screen.resolutions;
    }

}
