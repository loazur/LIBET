using UnityEngine;

public class S_TPInteraction : MonoBehaviour, SI_Interactable
{
    //~ Gestion de la téléportation
    [Header("Gestion de la téléportation")]
    [SerializeField] private Transform transformToTP;

    [Header("Traduction")]
    [SerializeField] private string interactTextFrench;
    [SerializeField] private string interactTextEnglish;
    
    private string interactText = "not_set"; // Texte à afficher

    void Start() //& Initialize la montre
    {
        UpdateInteractText(); // Setup
        
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        playerTransform.gameObject.transform.position = transformToTP.position;
        playerTransform.gameObject.transform.rotation = transformToTP.rotation;
    }

    public string getInteractText() => interactText; //& Texte affiché sur l'UI
    public Transform getTransform() => transform; //& Position de la montre
    
    //!---------------------------------------------

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {   
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            interactText = interactTextFrench;
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            interactText = interactTextEnglish;
        }
    }
}
