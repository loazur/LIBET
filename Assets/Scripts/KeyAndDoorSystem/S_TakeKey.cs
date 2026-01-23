using UnityEngine;

/// <summary>
/// Script pour les objets clés ramassables.
/// Chaque clé est associée à une porte via doorID et possède un keyID unique.
/// </summary>
public class S_TakeKey : MonoBehaviour, SI_Interactable
{
    [Header("Configuration de la clé")]
    [Tooltip("L'ID de la porte que cette clé peut débloquer")]
    [SerializeField] private string doorID = "door_01";
    
    [Tooltip("L'ID unique de cette clé (doit être unique parmi les clés de la même porte)")]
    [SerializeField] private string keyID = "key_01";

    private string interactText = "not_set";

    //*-----------------------------------------------------*

    void Start()
    {
        UpdateInteractText();
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText;

        // S'autodétruit si déja trouvé
        if (S_KeyManager.instance.HasKey(doorID, keyID))
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        //& Se désabonner pour éviter les erreurs
        if (S_GameUserData.instance != null)
        {
            S_GameUserData.instance.OnLanguageChanged -= UpdateInteractText;
        }
    }

    //! Méthodes provenant de l'interface SI_Interactable
    //! =====================================================

    public void Interact(Transform playerTransform)
    {
        //& Enregistrer la clé dans le KeyManager
        if (S_KeyManager.instance != null)
        {
            S_KeyManager.instance.CollectKey(doorID, keyID);
        }
        else
        {
            Debug.LogWarning("[S_TakeKey] S_KeyManager.instance est null! Assurez-vous qu'un KeyManager existe dans la scène.");
        }

        //& Déclencher l'événement pour le système de quêtes
        if (S_GameManager.instance != null)
        {
            S_GameManager.instance.playerEvents.KeyCollected(gameObject, doorID, keyID);
        }

        //& Détruire l'objet clé
        Destroy(gameObject);
    }

    public string getInteractText()
    {
        return interactText;
    }

    public Transform getTransform()
    {
        return gameObject.transform;
    }

    //! =====================================================

    private void UpdateInteractText()
    {
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            interactText = "Prendre la clé";
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            interactText = "Take Key";
        }
    }
}
