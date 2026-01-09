using UnityEngine;

public class S_MedicineInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion du medicament
    [Header("Gestion du medicament")]
    [SerializeField] private float percentageLucidityJaugeAward = 5; // Pourcentage de lucidité récupérer
    [SerializeField] private string interactText = "not_set"; // Texte à afficher

    void Start()
    {
        UpdateInteractText(); // Setup
        
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        S_AlzheimerEventsManager.instance.RecoverLucidity(percentageLucidityJaugeAward); // Gain de lucidité
        Debug.Log($"Délicieux! {percentageLucidityJaugeAward}% de lucidité récupéré!");

        // Notifier le manager qu'un médicament a été mangé
        S_MedicinesManager.instance.OnMedicineEatenByPlayer(gameObject);

        Destroy(gameObject); // Détruit le médicament
    }

    public string getInteractText() //& Texte affiché sur l'UI
    {
        return interactText;
    }

    public Transform getTransform() //& Position du médicament
    {
        return gameObject.transform;
    }

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            interactText = "Manger";
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            interactText = "Eat";
        }
    }
}
