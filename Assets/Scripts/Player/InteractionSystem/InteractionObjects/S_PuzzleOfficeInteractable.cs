using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_PuzzleOfficeInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion du casse tête
    [Header("Gestion du casse tête")]
    [SerializeField] private string interactText = "not_set"; // Texte à afficher

    [SerializeField] private bool[] password = new bool[8];

    [Header("Couleurs de la outline des pistons")]
    [SerializeField] private Color pistonActionner;
    [SerializeField] private Color pistonDeactionner;

    // Gère les états de chaque piston en fonction de son ID
    private Dictionary<string, bool> pistonsState = new Dictionary<string, bool>
    {
        {"Piston1", false},
        {"Piston2", false},
        {"Piston3", false},
        {"Piston4", false},
        {"Piston5", false},
        {"Piston6", false},
        {"Piston7", false},
        {"Piston8", false}
    };


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

    public string getInteractText() => interactText; //& Texte affiché sur l'UI
    public Transform getTransform() => transform; //& Position de la montre

    //!---------------------------------------------

    private void HandleItem3DClick(RaycastHit hit) //& Gère le clique sur chaque piston
    {
        // Récupérer le composant sur la partie cliquée
        S_Item3DInteractable interactable = hit.collider.GetComponent<S_Item3DInteractable>();
        
        if (interactable != null)
        {
            // Gérer les interactions spécifiques au puzzle
            HandlePuzzleInteraction(interactable);
        }
    }

    private void HandlePuzzleInteraction(S_Item3DInteractable part) //& Gère l'interaction avec le puzzle
    {
        string partID = part.interactionID;
        Outline partOutline = part.GetComponent<Outline>(); // TODO - Affichage d'une outline

        if (pistonsState[partID] == false) 
        {
            pistonsState[partID] = true;
            //partOutline.OutlineColor = pistonDeactionner;

            S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.buttonPushed, S_FMODEvents.instance.target.position);

            //Debug.Log($"{partID} actionné!");
        }
        else
        {
            pistonsState[partID] = false;
            //partOutline.OutlineColor = pistonActionner;

            S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.buttonUnpushed, S_FMODEvents.instance.target.position);

            //Debug.Log($"{partID} déactionné!");
        }

        CheckPuzzleDone();
    }

    private void CheckPuzzleDone()
    {
        // Vérifie si le puzzle est terminé
        for (int i = 0; i < pistonsState.Count; ++i)
        {
            if (pistonsState.ElementAt(i).Value != password[i]) // Si différent du mot de passe
            {
                return;
            }
        }

        TriggerPasswordFound();
    }

    private void TriggerPasswordFound()
    {
        Debug.Log("Mot de passe du puzzle trouvé");

        S_Item3DViewer.instance.TriggerEndExamine();
        Destroy(gameObject);
    }

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
