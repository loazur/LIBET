using UnityEngine;

public class S_Obj_InteractHandler : MonoBehaviour
{
    [SerializeField] private PlayerInteractedEventChannel_SO playerInteractedEvent;
    [SerializeField] private SO_ObjectiveDefinition objective;

    private void OnEnable()
    {
        playerInteractedEvent.RegisterListener(OnPlayerInteracted);
    }

    private void OnDisable()
    {
        playerInteractedEvent.UnregisterListener(OnPlayerInteracted);
    }

    private void OnPlayerInteracted(SI_Interactable interactable)
    {
        // Vérifie si c’est l’objet attendu pour cet objectif
        if (objective.targetId == interactable.name)
        {
            Debug.Log($"Objectif '{objective.description}' accompli !");
            // ➕ Ici, tu mettras la logique pour valider l’objectif
        }
    }
}
