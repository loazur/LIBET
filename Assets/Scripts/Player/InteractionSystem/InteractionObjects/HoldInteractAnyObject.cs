using UnityEngine;

/**
 * Interaction simple pour détecter un maintien de touche E sur n'importe quel objet
 * Déclenche un événement quand le joueur maintient E sur cet objet
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Sunday, January 12th, 2026.
 * @global
 */
public class HoldInteractAnyObject : MonoBehaviour, SI_Interactable
{
    [Header("Interaction Settings")]
    [SerializeField] private string interactTextFrench = "Maintenir E";
    [SerializeField] private string interactTextEnglish = "Hold E";

    private bool hasInteractedWithObject = false;

    //! Méthodes provenant de l'interface SI_Interactable

    // ~ Méthode qui est activée quand on interagit avec l'objet
    public void Interact(Transform playerTransform)
    {
        if (hasInteractedWithObject) return;

        hasInteractedWithObject = true;

        Debug.Log($"<color=red>[HoldInteractAnyObject] Player interacted with {gameObject.name}!</color>");

        // Notifier le système de quêtes
        if (S_GameManager.instance != null)
        {
            S_GameManager.instance.playerEvents.PlayerHoldInteractedWithAnyObject(gameObject);
            Debug.Log($"<color=red>[HoldInteractAnyObject] Event PlayerHoldInteractedWithAnyObject triggered for {gameObject.name}</color>");
        }
        else
        {
            Debug.LogWarning($"<color=red>[HoldInteractAnyObject] S_GameManager.instance is null!</color>");
        }
    }

    public string getInteractText()
    {
        if (S_GameUserData.instance == null)
        {
            return interactTextFrench; // Fallback
        }

        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            return interactTextFrench;
        }
        else
        {
            return interactTextEnglish;
        }
    }

    public Transform getTransform()
    {
        return gameObject.transform;
    }

    //! -------------------------------------------------------

    public void ResetInteraction()
    {
        hasInteractedWithObject = false;
        Debug.Log($"<color=red>[HoldInteractAnyObject] Interaction reset for {gameObject.name}</color>");
    }
}
