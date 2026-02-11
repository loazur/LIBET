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

    void OnDestroy()
    {
        // Nettoyer l'abonnement
        if (S_Item3DViewer.instance != null)
        {
            S_Item3DViewer.instance.OnItem3DClicked -= HandleItem3DClick;
        }
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        S_Item3DViewer.instance.TriggerExamine(transform);
        
        // S'abonner à l'événement de clic sur le modèle 3D
        S_Item3DViewer.instance.OnItem3DClicked += HandleItem3DClick;
    }

    private void HandleItem3DClick(RaycastHit hit)
    {
        // Récupérer le composant sur la partie cliquée
        S_Item3DInteractable interactable = hit.collider.GetComponent<S_Item3DInteractable>();
        
        if (interactable != null)
        {
            // Gérer les interactions spécifiques au puzzle
            HandlePuzzleInteraction(interactable.interactionID);
        }
    }

    private void HandlePuzzleInteraction(string partID)
    {
        switch (partID)
        {
            case "Piston1":
                Debug.Log("Piston1 pressé");
                // Logique du puzzle
                break;
            case "Piston2":
                Debug.Log("Piston2 actionné");
                // Logique du puzzle
                break;
            // ... autres parties
            default:
                Debug.Log("accaca");
                break;
        }
    }

    public string getInteractText() => interactText; //& Texte affiché sur l'UI
    public Transform getTransform() => transform; //& Position de la montre

    //!---------------------------------------------

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            interactText = "Examiner";
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            interactText = "Examine";
        }
    }
}
