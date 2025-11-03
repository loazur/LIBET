using System.Collections;
using UnityEngine;

public class S_DoorInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de la porte
    [Header("Gestion de la porte")]
    [SerializeField] private float rotationSpeed = 1f; // Vitesse d'ouverture
    [SerializeField] private float rotationAmount = 90f; // L'angle d'ouverture
    [SerializeField] private bool isRotatingDoor = true;

    private string interactText = "Ouvrir"; // Texte affiché sur l'UI
    private bool isOpen = false;
    
    // Porte qui rotate
    private float forwardDirection = 0f;
    private Vector3 startRotationVec;
    private Vector3 forward;

    private Coroutine animationCoroutine; // Coroutine d'ouverture

    void Start() //& INITIALISATION VARIABLES
    {
        startRotationVec = transform.rotation.eulerAngles;
        forward = transform.right;
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform) //& Ouverture de la porte
    {
        if (!isOpen)
        {
            Open(playerTransform.position);
            interactText = "Fermer";
        }
        else
        {
            Close();
            interactText = "Ouvrir";
        }

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
        }
    }

    private IEnumerator DoRotationOpen(float forwardAmount)
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
            time += Time.deltaTime * rotationSpeed;
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
        }
    }

    private IEnumerator DoRotationClose()
    {
        Quaternion startRotationQuat = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(startRotationVec);

        isOpen = false;

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotationQuat, endRotation, time);
            yield return null;
            time += Time.deltaTime * rotationSpeed;
        }

    }

}
