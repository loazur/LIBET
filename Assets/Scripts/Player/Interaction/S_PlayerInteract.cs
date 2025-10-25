using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerInteract : MonoBehaviour
{
    //~ Gestion des interactions
    [SerializeField] private float interactRange;
    private InputAction interactAction;

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
        List<SI_Interactable> interactableList = new List<SI_Interactable>();
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange); // Récupère tout les colliders autour du joueur

        foreach (Collider collider in colliderArray) // On récupère tout les colliders autour du joueur
        {
            if (collider.TryGetComponent(out SI_Interactable interactable)) // On regarde si c'est un NPC
            {
                interactableList.Add(interactable);
            }
        }


        // Recherche l'interaction la plus proche
        SI_Interactable closestInteractable = null;

        foreach(SI_Interactable interactable in interactableList)
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

    


}
