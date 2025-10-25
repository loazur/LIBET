using UnityEngine;
using TMPro;

public class S_PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private S_PlayerInteract playerInteract;
    [SerializeField] private TextMeshProUGUI interactText;

    void Update()
    {
        if (playerInteract.GetInteractableObject() != null) // Est à portée d'une interaction
        {
            Show(playerInteract.GetInteractableObject());
        }
        else
        {
            Hide();
        }
    }

    //! --------------- Fonctions privés ---------------

    private void Show(S_NPCInteractable npcInteractable)
    {
        containerGameObject.SetActive(true);
        interactText.text = npcInteractable.getInteractText();
    }
    
    private void Hide()
    {
        containerGameObject.SetActive(false);
    }

}
