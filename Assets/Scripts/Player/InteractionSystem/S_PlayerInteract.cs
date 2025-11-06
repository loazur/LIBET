using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerInteract : MonoBehaviour
{
    //~ Gestion des interactions
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactRange = 2f;
    private InputAction interactAction;
    private bool areInteractionsEnabled = true;
    private S_ItemInteraction holdingItem = null;

    void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Update() //& PAS PHYSICS
    {
        if (interactAction.WasReleasedThisFrame())
        {
            SI_Interactable interactable = GetInteractableObject();

            if (interactable != null)
            {
                interactable.Interact(transform);
            }

        }
    }

    //! --------------- Fonctions privés ---------------

    public SI_Interactable GetInteractableObject() //& Recherche l'interaction la plus proche et la retourne
    {
        if (!canInteract()) // Si désactivé
        {
            return null;
        }

        //~ Raycast droit devant (en priorité)
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactRange))
        {
            if (hit.collider.TryGetComponent(out SI_Interactable interactableHit))
            {
                return interactableHit; // Priorité à ce que le joueur regarde
            }
        }
        
        //~ Utilisation de la méthode avec la sphere (si jamais le raycast n'a rien donné)
        List<SI_Interactable> interactableList = new List<SI_Interactable>();
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange); // Récupère tout les colliders autour du joueur

        foreach (Collider collider in colliderArray) // On récupère tout les colliders autour du joueur
        {
            if (collider.TryGetComponent(out SI_Interactable interactable)) // On regarde si c'est un NPC
            {
                // Vérifie en fonction de si l'objet est devant le joueur
                float dot = Vector3.Dot(transform.forward, (interactable.getTransform().position - transform.position).normalized);

                if (dot > 0.5f) // 60° donc devant le joueur
                {
                    interactableList.Add(interactable); // On peux intéragir avec
                }

            }
        }

        // Recherche l'interaction la plus proche
        SI_Interactable closestInteractable = null;

        foreach (SI_Interactable interactable in interactableList)
        {
            if (closestInteractable == null)
            {
                closestInteractable = interactable;
            }
            else
            {
                if (Vector3.Distance(transform.position, interactable.getTransform().position) <
                Vector3.Distance(transform.position, closestInteractable.getTransform().position))
                {
                    // Le plus proche
                    closestInteractable = interactable;
                }
            }
        }

        return closestInteractable; // Retourne l'interaction la plus proche
    }

    //? ------------------------------------------------

    public bool canInteract() //& Retourne si les interactions sont actif ou pas
    {
        return areInteractionsEnabled;
    }

    public void setInteractionEnabled(bool isEnabled) //& Active/Désactive les interactions
    {
        areInteractionsEnabled = isEnabled;
    }

    //? ------------------------------------------------

    public bool isHoldingItem() //& Si le joueur tient un item
    {
        return holdingItem;
    }
    
    public void setHoldingItem(S_ItemInteraction itemHolded) //& Permet d'activer si il tiens un item
    {
        holdingItem = itemHolded;
    }

}
