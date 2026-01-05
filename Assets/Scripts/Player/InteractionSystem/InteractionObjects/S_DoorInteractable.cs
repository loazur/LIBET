using System.Collections;
using UnityEngine;
using FMODUnity;

public class S_DoorInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de la porte
    [Header("Type de porte")]
    [SerializeField] protected bool isRotatingDoor = true;
    [SerializeField] protected float speed = 1f; // Vitesse d'ouverture/fermeture

    protected string interactText = "not_set"; // Texte affiché sur l'UI
    protected bool isOpen = false;
    
    //~ Porte Rotative
    [Header("Gestion de la porte rotative")]
    [SerializeField] protected float rotationAmount = 90f; // L'angle d'ouverture
    protected float forwardDirection = 0f;

    //~ Porte coulissante
    [Header("Gestion de la porte coulissante")]
    [SerializeField] protected Vector3 slideDirection = Vector3.back;
    [SerializeField] protected float slideAmount = 1.9f;

    protected Vector3 startPositionVec;
    protected Vector3 startRotationVec;
    protected Vector3 forward;

    protected Coroutine animationCoroutine; // Coroutine d'ouverture

    protected virtual void Start() //& INITIALISATION VARIABLES
    {
        UpdateInteractText(); // Setup

        startRotationVec = transform.rotation.eulerAngles;
        forward = transform.right;
        startPositionVec = transform.position;

        S_GameSettingsData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    protected virtual void OnDestroy()
    {
        if (S_GameSettingsData.instance != null)
        {
            S_GameSettingsData.instance.OnLanguageChanged -= UpdateInteractText;
        }
    }


    //! Méthodes provenant de l'interface SI_Interactable

    public virtual void Interact(Transform playerTransform) //& Ouverture de la porte
    {
        if (!isOpen)
        {
            Open(playerTransform.position);
        }
        else
        {
            Close();
        }

        UpdateInteractText();
    }

    public virtual string getInteractText() //& Texte de la porte
    {
        return interactText;
    }

    public virtual Transform getTransform() //& Position de la porte
    {
        return gameObject.transform;
    }


    //! --------------- Fonctions protégées ---------------

    protected virtual void Open(Vector3 playerPosition) //& Gére le coroutine d'ouverture
    {
        if (!isOpen)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            if (isRotatingDoor) // Si porte rotative
            {
                // Ce qui permettra de vérifier de quel coté de la porte le joueur est
                float dot = Vector3.Dot(forward, (playerPosition - transform.position).normalized);
                animationCoroutine = StartCoroutine(DoRotationOpen(dot));
            }
            else
            {
                animationCoroutine = StartCoroutine(DoSlidingOpen());
            }

            // Son d'ouverture
            S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.doorOpening, transform.position);
        }
    }

    protected virtual IEnumerator DoRotationOpen(float forwardAmount) //& Ouverture porte rotative
    {
        Quaternion startRotationQuat = transform.rotation;
        Quaternion endRotation;

        if (forwardAmount >= forwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0, startRotationVec.y + rotationAmount, 0));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0, startRotationVec.y - rotationAmount, 0));
        }

        isOpen = true;

        // Notifier le système de quêtes
        if (S_GameManager.instance != null)
        {
            S_GameManager.instance.playerEvents.DoorOpened(gameObject);
        }

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotationQuat, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    protected virtual IEnumerator DoSlidingOpen() //& Ouverture porte coulissante
    {
        Vector3 endPosition = startPositionVec + slideAmount * slideDirection;
        Vector3 startPosition = transform.position;
        
        float time = 0;
        isOpen = true;

        // Notifier le système de quêtes
        if (S_GameManager.instance != null)
        {
            S_GameManager.instance.playerEvents.DoorOpened(gameObject);
        }
        
        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    protected virtual void Close() //& Gére le coroutine de fermeture
    {
        if (isOpen)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            if (isRotatingDoor) // Si porte rotative
            {
                animationCoroutine = StartCoroutine(DoRotationClose());
            }
            else
            {
                animationCoroutine = StartCoroutine(DoSlidingClose());
            }

            // Son de fermeture
            S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.doorClosing, transform.position);
        }
    }

    protected virtual IEnumerator DoRotationClose() //& Fermeture porte rotative
    {
        Quaternion startRotationQuat = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(startRotationVec);

        isOpen = false;

        // Notifier le système de quêtes
        if (S_GameManager.instance != null)
        {
            S_GameManager.instance.playerEvents.DoorClosed(gameObject);
        }

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotationQuat, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

    }

    protected virtual IEnumerator DoSlidingClose() //& Fermeture porte coulissante
    {
        Vector3 endPosition = startPositionVec;
        Vector3 startPosition = transform.position;

        float time = 0;
        isOpen = false;

        // Notifier le système de quêtes
        if (S_GameManager.instance != null)
        {
            S_GameManager.instance.playerEvents.DoorClosed(gameObject);
        }

        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }
    

    protected virtual void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (!isOpen) // Si fermer
        {
            if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.French)
            {
                interactText = "Ouvrir";
            }
            else if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.English)
            {
                interactText = "Open";
            }
        }
        else // Si ouverte
        {
            if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.French)
            {
                interactText = "Fermer";
            }
            else if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.English)
            {
                interactText = "Close";
            }
        }
    }

}
