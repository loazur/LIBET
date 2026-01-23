using System;
using NUnit.Framework;
using UnityEngine;

public class S_TPInteraction : MonoBehaviour, SI_Interactable
{
    //~ Gestion de la téléportation
    [Header("Gestion de la téléportation")]
    [SerializeField] private Transform transformToTP;

    [Header("Configuration du verrouillage")]
    [Tooltip("Pour vérouiler la téléportation comme une porte")]
    [SerializeField] private bool isLocked = false;

    [Tooltip("L'ID unique de ce point de téléportation (doit correspondre au doorID des clés)")]
    [SerializeField] private string teleportID = "teleport_01";

    [Header("Traduction")]
    [SerializeField] private string interactTextFrench;
    [SerializeField] private string interactTextEnglish;
    
    private string interactText = "not_set"; // Texte à afficher
    private bool isUnlocked = false;
    
    // *---------------------------------------------------------------------
    void Start() //& Initialize la montre
    {
        UpdateInteractText(); // Setup
        
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue

        SubscribeToKeyManager();

        // Possède déjà le droit de ce TP
        if (S_KeyManager.instance.HasAllKeys(teleportID, 1))
        {
            isUnlocked = true;
        }
    }

    void OnDestroy()
    {
        if (isLocked && S_KeyManager.instance != null)
        {
            S_KeyManager.instance.OnKeyCollected -= OnKeyCollected;
        }
    }
    // *---------------------------------------------------------------------

    
    //! Méthodes provenant de l'interface SI_Interactable
    //! =====================================================
    public void Interact(Transform playerTransform)
    {
        bool isFrench = S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French;


        if (isLocked && !isUnlocked)
        {
            //& Point de téléportation verrouillé - afficher si la clé est collectée
            int currentKeys = S_KeyManager.instance != null ? S_KeyManager.instance.GetCollectedKeyCount(teleportID) : 0;
            
            if (S_KeyManager.instance != null && S_KeyManager.instance.HasAllKeys(teleportID, 1))
            {
                UnlockTeleportation(); //& Accepte la TP
            }
            else
            {
                OnLockedInteraction(); //& Refuse la TP
                return;
            }
        }

        //& Passe pas ici si c'est bloquer
        playerTransform.gameObject.transform.position = transformToTP.position;
        playerTransform.gameObject.transform.rotation = transformToTP.rotation;
    }

    public string getInteractText() => interactText; //& Texte affiché sur l'UI
    public Transform getTransform() => transform; //& Position de la montre

    //! =====================================================


    private void SubscribeToKeyManager() //& S'abonner à l'événement de collecte de clé
    {
        if (isLocked && S_KeyManager.instance != null)
        {
            S_KeyManager.instance.OnKeyCollected += OnKeyCollected;
        }
    }

    private void OnKeyCollected(string collectedTeleportID, string keyID)
    {
        if (collectedTeleportID == teleportID)
        {
            UpdateInteractText();
        }
    }

    private void UnlockTeleportation() //& Déverrouille la téléportation
    {
        isUnlocked = true;
        
        // Jouer un son de déverrouillage 
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.doorUnlock, transform.position);

        UpdateInteractText();
    }

    private void OnLockedInteraction()
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.doorLocked, transform.position);
    }

    //!---------------------------------------------

    //* Langue
    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {   
        bool isFrench = S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French;

        if (isLocked && !isUnlocked)
        {
            //& Point de téléportation verrouillé - afficher si la clé est collectée
            int currentKeys = S_KeyManager.instance != null ? S_KeyManager.instance.GetCollectedKeyCount(teleportID) : 0;
            
            if (isFrench)
            {
                interactText = $"Verrouillé ({currentKeys}/1 clé)";
            }
            else
            {
                interactText = $"Locked ({currentKeys}/1 key)";
            }
        }
        else
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
            interactText = interactTextFrench;
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
            interactText = interactTextEnglish;
            }
        }
    
        
    }
}
