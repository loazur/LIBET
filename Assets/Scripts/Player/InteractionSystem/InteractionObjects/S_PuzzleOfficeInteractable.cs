using UnityEngine;

public class S_PuzzleOfficeInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion du casse tête
    [Header("Gestion du casse tête")]
    [SerializeField] private string interactText = "not_set"; // Texte à afficher

    void Start() //& Initialize la montre
    {
        UpdateInteractText(); // Setup

        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        S_Item3DViewer.instance.TriggerExamine(transform);
    }

    public string getInteractText() => interactText; //& Texte affiché sur l'UI
    public Transform getTransform() => transform; //& Position de la montre

    //!---------------------------------------------

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            interactText = "Dormir";
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            interactText = "Sleep";
        }
    }
}
