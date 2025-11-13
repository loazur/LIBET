using UnityEngine;
using UnityEngine.UI;

public class S_VideoSettingsData : MonoBehaviour
{
    public static S_VideoSettingsData instance;

    [Header("Gestion de l'UI ")]
    [SerializeField] private TMPro.TMP_Dropdown dropdownResolution;
    [SerializeField] private TMPro.TMP_Dropdown dropdownFullscreen; 
    [SerializeField] private TMPro.TMP_Dropdown dropdownParticlesEffects; 
    [SerializeField] private Slider sliderFPSMax; 
    [SerializeField] private Toggle toggleVSync; 
    
    //! Valeurs par défauts
    //TODO

    //! Actuellement utilisé
    //TODO


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        LoadData();
    }

    //?------------------------------------- SETS

    //TODO

    //?------------------------------------- RESETS

    //TODO

    //?------------------------------------- EVENTS


    //! ------------------------------------ SAVES/LOADS

    public void SaveData() //& Sauvegarde des données
    {
        // Met à jour les préferences
        //TODO

        // Les sauvegarde dans PlayerPrefs
        PlayerPrefs.Save();
    }

    public void LoadData() //& Charge les données
    {
        //Charge les données si elles sont présentes sinon charge les valeurs par défaut
        //TODO   
    }

}
