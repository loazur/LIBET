using System;
using UnityEngine;
using UnityEngine.UI;

public class S_JaugeCheckerInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de l'UI
    [Header("UI")]
    [SerializeField] private float maxDistance = 3f; // Distance maximale avant désactivation auto
    [SerializeField] private GameObject jaugeCheckerUI;
    [SerializeField] private Slider jaugeSlider;
    [SerializeField] private float smoothSliderChange = 0.2f;

    //~ Gestion de la jauge checker
    [Header("Gestion du gradient")]
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fillImage;

    private bool isUsed = false;

    private Transform playerTransform; // Référence au joueur
    private string interactText = "not_set"; // Texte à afficher

    void Start() //& Initialize la montre
    {
        UpdateInteractText(); // Setup
        
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    void Update() //& Gère la mise à jour de l'affichage
    {
        if (isUsed)
        {
            UpdateJaugeChecker();
            CheckPlayerDistance();
        }
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        if (!isUsed) // Activer la montre
        {
            this.playerTransform = playerTransform; // Sauvegarder la référence au joueur
            EnableJaugeChecker();
        }
        else // Désactiver la montre
        {
            DisableJaugeChecker();
        }

        UpdateInteractText();
    }

    public string getInteractText() => interactText; //& Texte affiché sur l'UI
    public Transform getTransform() => transform; //& Position du jauge checker
    
    //!---------------------------------------------

    private void EnableJaugeChecker() //& Active la jauge checker
    {
        if (isUsed) return;

        isUsed = true;
        jaugeCheckerUI.SetActive(true); // Affichage de l'UI
    }

    private void DisableJaugeChecker() //& Désactive la jauge checker
    {
        if (!isUsed) return;

        isUsed = false;
        jaugeCheckerUI.SetActive(false);

        playerTransform = null; // Réinitialiser la référence
        
        UpdateInteractText(); // Mettre à jour le texte d'interaction
    }

    private void CheckPlayerDistance() //& Vérifie la distance avec le joueur
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > maxDistance) // Trop éloigné
        {
            DisableJaugeChecker();
        }
    }

    private void UpdateJaugeChecker()
    {
        jaugeSlider.value = Mathf.Lerp(jaugeSlider.value, S_AlzheimerEventsManager.instance.Lucidity / 100, smoothSliderChange); // Met la valeur de la jauge du slider egale à la lucidité
        fillImage.color = gradient.Evaluate(jaugeSlider.value);
    }

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (!isUsed)
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                interactText = "Activer";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                interactText = "Enable";
            }
        }
        else
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                interactText = "Désactiver";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                interactText = "Disable";
            }
        }
    }
}
