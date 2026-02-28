using System.Collections.Generic;
using UnityEngine;

public class S_PlayerInteract : MonoBehaviour
{
    //! S_PlayerInteract gère la détection des interactions et l'activation de cet meme interaction

    //~ Gestion des interactions
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactRange = 2f;
    private bool areInteractionsEnabled = true;
    private S_ItemInteraction holdingItem = null;

    // Pour gérer l'interaction maintenue
    private HoldToInteract currentHoldingItem = null;

    // Pour gérer les minijeux
    private S_AbstractMinigame currentMinigame = null;
    private SI_Interactable currentMinigameInteractable = null;

    void Update() //& PAS PHYSICS
    {
        SI_Interactable interactable = GetInteractableObject();

        // Vérifier si c'est un objet qui nécessite de maintenir
        if (interactable != null && interactable.getTransform().TryGetComponent(out HoldToInteract holdingScript))
        {
            // Si le joueur commence à appuyer
            if (S_UserInput.instance.InteractAction.WasPressedThisFrame())
            {
                currentHoldingItem = holdingScript;
                currentHoldingItem.holdTimer = currentHoldingItem.howLongToHold;
            }

            // Si le joueur maintient le bouton
            if (S_UserInput.instance.InteractAction.IsPressed() && currentHoldingItem != null)
            {
                currentHoldingItem.holdTimer -= Time.deltaTime;

                // Si le timer est écoulé, déclencher l'interaction
                if (currentHoldingItem.holdTimer <= 0)
                {
                    // Déclencher l'événement pour le système de quêtes
                    if (S_GameManager.instance != null)
                    {
                        S_GameManager.instance.playerEvents.PlayerHoldInteracted(
                            interactable.getTransform().gameObject.name,
                            interactable.getTransform().gameObject.tag
                        );
                    }

                    interactable.Interact(transform);
                    currentHoldingItem = null; // Reset
                }
            }

            // Si le joueur relâche le bouton avant la fin
            if (S_UserInput.instance.InteractAction.WasReleasedThisFrame())
            {
                if (currentHoldingItem != null)
                {
                    currentHoldingItem.holdTimer = currentHoldingItem.howLongToHold; // Reset
                    currentHoldingItem = null;
                }
            }
        }
        else if (interactable != null && interactable.getTransform().TryGetComponent(out S_AbstractMinigame minigame)) // Minijeu
        {
            if (S_UserInput.instance.InteractAction.WasPressedThisFrame())
            {
                if (interactable != null)
                {
                    // Stocker les références
                    currentMinigame = minigame;
                    currentMinigameInteractable = interactable;
                    
                    // S'abonner à l'événement
                    minigame.OnMinigameWin += OnMinigameCompleted;
                    
                    // Lancer le minijeu
                    minigame.TriggerMinigame();
                }
            }
        }
        else // Interaction normale (pas besoin de maintenir)
        {
            if (S_UserInput.instance.InteractAction.WasPressedThisFrame())
            {
                if (interactable != null)
                {
                    interactable.Interact(transform);
                }
            }

            // Reset si on ne vise plus un objet à maintenir
            if (currentHoldingItem != null)
            {
                currentHoldingItem.holdTimer = currentHoldingItem.howLongToHold;
                currentHoldingItem = null;
            }
        }
    }

    private void OnMinigameCompleted()
    {
        if (currentMinigameInteractable != null && currentMinigame != null)
        {
            // Appeler l'interaction
            currentMinigameInteractable.Interact(transform);

            // Notifier le système de quêtes (passe le GameObject du minijeu)
            if (S_GameManager.instance != null)
            {
                S_GameManager.instance.playerEvents.MinigameCompleted(currentMinigame.gameObject);
            }
            
            // Se désabonner pour éviter les fuites mémoire
            currentMinigame.OnMinigameWin -= OnMinigameCompleted;
            
            // Reset
            currentMinigame = null;
            currentMinigameInteractable = null;
        }
    }

    void OnDestroy()
    {
        // Nettoyage si le script est détruit pendant un minijeu
        if (currentMinigame != null)
        {
            currentMinigame.OnMinigameWin -= OnMinigameCompleted;
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

        //! Détection d'interaction le plus proche (et à travers les murs)
        /*
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
        */
        return null;
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

    public S_ItemInteraction GetHoldingItem()
    {
        return holdingItem;
    }

    public float GetHoldProgress() //& Retourne le progrès de l'interaction maintenue (0 à 1)
    {
        if (currentHoldingItem == null)
            return 0f;

        return 1f - (currentHoldingItem.holdTimer / currentHoldingItem.howLongToHold);
    }
}
