using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerInteract : MonoBehaviour
{
    private InputAction interactAction;

    void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Update() //& PAS PHYSICS
    {
        if (interactAction.WasReleasedThisFrame())
        {
            float interactRange = 2f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

            foreach (Collider collider in colliderArray) // On récupère tout les colliders autour du joueur
            {
                if (collider.TryGetComponent(out S_NPCInteractable npcInteractable)) // On regarde si c'est un NPC
                {
                    npcInteractable.Interact();
                }
            }
        }
    }

    //! --------------- Fonctions privés ---------------
    
    public S_NPCInteractable GetInteractableObject()
    {
        List<S_NPCInteractable> npcInteractableList = new List<S_NPCInteractable>();

        float interactRange = 4f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray) // On récupère tout les colliders autour du joueur
        {
            if (collider.TryGetComponent(out S_NPCInteractable npcInteractable)) // On regarde si c'est un NPC
            {
                npcInteractableList.Add(npcInteractable);
            }
        }


        // Recherche du NPC le plus proche
        S_NPCInteractable closestNPCInteractable = null;
        foreach(S_NPCInteractable npcInteractable in npcInteractableList)
        {
            if (closestNPCInteractable == null)
            {
                closestNPCInteractable = npcInteractable;
            }
            else
            {
                if (Vector3.Distance(transform.position, npcInteractable.transform.position) < 
                Vector3.Distance(transform.position, closestNPCInteractable.transform.position))
                {
                    // Closer
                    closestNPCInteractable = npcInteractable;
                }
            }
        }

        return closestNPCInteractable;

    }

    


}
