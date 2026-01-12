using UnityEngine;
using System.Collections;

public class S_CupboardInteractable : MonoBehaviour, SI_Interactable
{
     //~ Gestion du placard
    [Header("Gestion du placard")]
    [SerializeField] private GameObject objectToTurn;
    [SerializeField] protected float rotationAmount = 90f; // L'angle d'ouverture
    [SerializeField] protected float speed = 1f; // Vitesse d'ouverture/fermeture
    [SerializeField] protected bool rightHandle = false;
    protected float forwardDirection = 0f;

    protected Vector3 startPositionVec;
    protected Vector3 startRotationVec;
    protected Vector3 forward;

    protected string interactText = "not_set"; // Texte affiché sur l'UI
    protected bool isOpen = false;

    protected Coroutine animationCoroutine; // Coroutine d'ouverture


    protected virtual void Start() //& INITIALISATION VARIABLES
    {
        UpdateInteractText(); // Setup

        startRotationVec = objectToTurn.transform.rotation.eulerAngles;
        forward = objectToTurn.transform.right;
        startPositionVec = objectToTurn.transform.position;

        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    protected virtual void OnDestroy()
    {
        if (S_GameUserData.instance != null)
        {
            S_GameUserData.instance.OnLanguageChanged -= UpdateInteractText;
        }
    }


    //! Méthodes provenant de l'interface SI_Interactable

    public virtual void Interact(Transform playerTransform) //& Ouverture du placard
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

    public virtual string getInteractText() //& Texte du placard
    {
        return interactText;
    }

    public virtual Transform getTransform() //& Position du placard
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

            // Ce qui permettra de vérifier de quel coté de la porte le joueur est
            animationCoroutine = StartCoroutine(DoRotationOpen());

            // Notifier le système de quêtes
            if (S_GameManager.instance != null)
            {
                S_GameManager.instance.playerEvents.CupboardOpened(gameObject);
                Debug.Log("[S_CupboardInteractable] Cupboard opened event sent");
            }
            
            // Son d'ouverture
            S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.doorOpening, transform.position);
        }
    }

    protected virtual IEnumerator DoRotationOpen() //& Ouverture porte rotative
    {
        Quaternion startRotationQuat = objectToTurn.transform.rotation;
        Quaternion endRotation;

        // Garder les rotations X et Z existantes, modifier seulement Y
        if (!rightHandle)
        {
            endRotation = Quaternion.Euler(new Vector3(startRotationVec.x, startRotationVec.y + rotationAmount, startRotationVec.z));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(startRotationVec.x, startRotationVec.y - rotationAmount, startRotationVec.z));
        }

        isOpen = true;

        
        
        float time = 0;
        while (time < 1)
        {
            objectToTurn.transform.rotation = Quaternion.Slerp(startRotationQuat, endRotation, time);
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

            
            animationCoroutine = StartCoroutine(DoRotationClose());

            // Notifier le système de quêtes
            if (S_GameManager.instance != null)
            {
                S_GameManager.instance.playerEvents.CupboardClosed(gameObject);
                Debug.Log("[S_CupboardInteractable] Cupboard closed event sent");
            }
            

            // Son de fermeture
            S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.doorClosing, transform.position);
        }
    }

    protected virtual IEnumerator DoRotationClose() //& Fermeture porte rotative
    {
        Quaternion startRotationQuat = objectToTurn.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(startRotationVec);

        isOpen = false;

        

        float time = 0;
        while (time < 1)
        {
            objectToTurn.transform.rotation = Quaternion.Slerp(startRotationQuat, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

    }
    

    protected virtual void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (!isOpen) // Si fermer
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                interactText = "Ouvrir";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                interactText = "Open";
            }
        }
        else // Si ouverte
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                interactText = "Fermer";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                interactText = "Close";
            }
        }
    }
}
