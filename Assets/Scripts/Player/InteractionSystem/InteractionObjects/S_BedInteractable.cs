using UnityEngine;

public class S_BedInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de la montre
    [Header("Gestion de la montre")]
    [SerializeField] private string interactText = "not_set"; // Texte à afficher

    //TODO Faire que le lit face changé de jour (si toutes les quetes sont terminées)

    void Start() //& Initialize la montre
    {
        UpdateInteractText(); // Setup
        
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        Debug.Log("DORMIR!!");
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
