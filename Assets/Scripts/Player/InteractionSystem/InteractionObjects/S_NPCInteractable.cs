using UnityEngine;

public class S_NPCInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de l'interaction du NPC
    [Header("Gestion du NPC et son dialogue")]
    [SerializeField] private S_DialogueTrigger dialogueTrigger;
    [SerializeField] private string interactText = "not_set"; // Texte à afficher en fonction du NPC

    void Start()
    {
        UpdateInteractText(); // Setup
        
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French) // Français
        {
            dialogueTrigger.TriggerDialogueFrench(); // Lance le dialogue du NPC en français
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English) // Anglais
        {
            dialogueTrigger.TriggerDialogueEnglish(); // Lance le dialogue du NPC en anglais
        }
    }

    public string getInteractText() //& Texte affiché sur l'UI
    {
        return interactText;
    }

    public Transform getTransform() //& Position du NPC
    {
        return gameObject.transform;
    }

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            interactText = "Parler";
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            interactText = "Talk";
        }
        
        
    }
}
