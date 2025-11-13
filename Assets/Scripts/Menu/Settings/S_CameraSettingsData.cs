using System;
using UnityEngine;
using UnityEngine.UI;

public class S_CameraSettingsData : MonoBehaviour
{
    public static S_CameraSettingsData instance;

    [Header("Gestion de l'UI ")]
    [SerializeField] private Slider sliderSensibilityMouse; //! Slider Settings Souris
    [SerializeField] private Slider sliderSensibilityController; //! Slider Settings Controller
    
    //! Valeurs par défauts
    private const float defaultSensibilityMouse = 100f;
    private const float defaultSensibilityController = 150f;
    private const bool defaultInverseXAxis = false;
    private const bool defaultInverseYAxis = false;

    //! Actuellement utilisé
    public float currentSensibilityMouse {get; private set;}
    public float currentSensibilityController {get; private set;}
    public bool currentInverseXAxis {get; private set;}
    public bool currentInverseYAxis {get; private set;}

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        LoadData();
    }

    //?------------------------------------- SETS

    public void setCurrentSensibilityMouse(float newSensibility)
    {
        if (currentSensibilityMouse == newSensibility)
            return;

        currentSensibilityMouse = newSensibility;
        sliderSensibilityMouse.value = currentSensibilityMouse;
        
    }

    public void setCurrentSensibilityController(float newSensibility)
    {
        if (currentSensibilityController == newSensibility)
            return;

        currentSensibilityController = newSensibility;
        sliderSensibilityController.value = currentSensibilityController;
    }

    public void setCurrentInverseXAxis(bool enabled)
    {
        if (currentInverseXAxis == enabled)
            return;

        currentInverseXAxis = enabled;
        //TODO changer UI
    }

    public void setCurrentInverseYAxis(bool enabled)
    {
        if (currentInverseYAxis == enabled)
            return;

        currentInverseYAxis = enabled;
        //TODO changer UI
    }

    //?------------------------------------- RESETS

    public void resetCurrentSensibilityMouse()
    {
        setCurrentSensibilityMouse(defaultSensibilityMouse);
    }

    public void resetCurrentSensibilityController()
    {
        setCurrentSensibilityController(defaultSensibilityController);
    }

    public void resetCurrentInverseXAxis()
    {
        setCurrentInverseXAxis(defaultInverseXAxis);
    }

    public void resetCurrentInverseYAxis()
    {
        setCurrentInverseYAxis(defaultInverseYAxis);
    }

    //?------------------------------------- EVENTS



    //! ------------------------------------ SAVES/LOADS

    public void SaveData() //& Sauvegarde des données
    {
        // Met à jour les préferences
        PlayerPrefs.SetFloat("MouseSensitivity", currentSensibilityMouse);
        PlayerPrefs.SetFloat("ControllerSensitivity", currentSensibilityController);
        PlayerPrefs.SetInt("InverseXAxis", Convert.ToInt32(currentInverseXAxis));
        PlayerPrefs.SetInt("InverseYAxis", Convert.ToInt32(currentInverseYAxis));

        // Les sauvegarde dans PlayerPrefs
        PlayerPrefs.Save();
    }

    public void LoadData() //& Charge les données
    {
        //Charge les données si elles sont présentes sinon charge les valeurs par défaut
        if (PlayerPrefs.HasKey("MouseSensitivity"))  //~ Language
            setCurrentSensibilityMouse(PlayerPrefs.GetFloat("MouseSensitivity"));
        else
            setCurrentSensibilityMouse(defaultSensibilityMouse);

        if (PlayerPrefs.HasKey("ControllerSensitivity")) //~ CameraShake
            setCurrentSensibilityController(PlayerPrefs.GetFloat("ControllerSensitivity"));
        else
            setCurrentSensibilityController(defaultSensibilityController);

        if (PlayerPrefs.HasKey("InverseXAxis")) //~ ATHSize
            setCurrentInverseXAxis(Convert.ToBoolean(PlayerPrefs.GetInt("InverseXAxis")));
        else
            setCurrentInverseXAxis(defaultInverseXAxis);

        if (PlayerPrefs.HasKey("InverseYAxis")) //~ TypingSpeed
            setCurrentInverseYAxis(Convert.ToBoolean(PlayerPrefs.GetInt("InverseYAxis")));
        else
            setCurrentInverseYAxis(defaultInverseYAxis);
    }

    
}
