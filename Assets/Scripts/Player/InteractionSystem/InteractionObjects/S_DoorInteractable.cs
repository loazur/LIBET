using System.Collections;
using UnityEngine;

public class S_DoorInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de la porte

    [Header("Type de porte")]
    [SerializeField] private bool isRotatingDoor = true;
    [SerializeField] private float speed = 1f; // Vitesse d'ouverture/fermeture

    private string interactText = "not_set"; // Texte affiché sur l'UI
    private bool isOpen = false;
    
    //~ Porte Rotative
    [Header("Gestion de la porte rotative")]
    [SerializeField] private float rotationAmount = 90f; // L'angle d'ouverture
    private float forwardDirection = 0f;

    //~ Porte coulissante
    [Header("Gestion de la porte coulissante")]
    [SerializeField] private Vector3 slideDirection = Vector3.back;
    [SerializeField] private float slideAmount = 1.9f;

    [Header("Gestion de l'audio")]
    [SerializeField] private AudioClip audioOpening;
    [SerializeField] private AudioClip audioClosing;

    private Vector3 startPositionVec;
    private Vector3 startRotationVec;
    private Vector3 forward;

    private Coroutine animationCoroutine; // Coroutine d'ouverture

    void Start() //& INITIALISATION VARIABLES
    {
        UpdateInteractText(); // Setup

        startRotationVec = transform.rotation.eulerAngles;
        forward = transform.right;
        startPositionVec = transform.position;

        S_GameSettingsData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }


    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform) //& Ouverture de la porte
    {
        if (!isOpen)
        {
            S_SoundFXManager.instance.PlaySoundFXClip(audioOpening, gameObject.transform, 1f);
            Open(playerTransform.position);
        }
        else
        {
            Close();
        }

        UpdateInteractText();
    }

    public string getInteractText() //& Texte de la porte
    {
        return interactText;
    }

    public Transform getTransform() //& Position de la porte
    {
        return gameObject.transform;
    }


    //! --------------- Fonctions privés ---------------

    private void Open(Vector3 playerPosition) //& Gére le coroutine d'ouverture
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
        }
    }

    private IEnumerator DoRotationOpen(float forwardAmount) //& Ouverture porte rotative
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

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotationQuat, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    private IEnumerator DoSlidingOpen() //& Ouverture porte coulissante
    {
        Vector3 endPosition = startPositionVec + slideAmount * slideDirection;
        Vector3 startPosition = transform.position;
        
        float time = 0;
        isOpen = true;
        
        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    private void Close() //& Gére le coroutine de fermeture
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
        }
    }

    private IEnumerator DoRotationClose() //& Fermeture porte rotative
    {
        Quaternion startRotationQuat = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(startRotationVec);

        isOpen = false;

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotationQuat, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

    }

    private IEnumerator DoSlidingClose() //& Fermeture porte coulissante
    {
        Vector3 endPosition = startPositionVec;
        Vector3 startPosition = transform.position;

        float time = 0;
        isOpen = false;

        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }
    

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
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
