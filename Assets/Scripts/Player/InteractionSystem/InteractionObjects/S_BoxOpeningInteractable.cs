using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_BoxOpeningInteractable : MonoBehaviour, SI_Interactable
{
    
    //~ Gestion du carton
    [Header("Gestion du carton")]
    [SerializeField] private GameObject flap1; // Volet 1 (avant)
    [SerializeField] private GameObject flap2; // Volet 2 (droite)
    [SerializeField] private GameObject flap3; // Volet 3 (arrière)
    [SerializeField] private GameObject flap4; // Volet 4 (gauche)
    [SerializeField] private float speed = 1f; // Vitesse d'ouverture/fermeture

    private Vector3 startRotationVec1;
    private Vector3 startRotationVec2;
    private Vector3 startRotationVec3;
    private Vector3 startRotationVec4;
    private string interactText = "not_set"; // Texte affiché sur l'UI
    private float rotationAmount = 195f; // L'angle d'ouverture
    private Coroutine animationCoroutine; // Coroutine d'ouverture


    void Start() //& INITIALISATION VARIABLES
    {
        UpdateInteractText(); // Setup

        // Sauvegarder les rotations LOCALES initiales
        if (flap1 != null) startRotationVec1 = flap1.transform.localRotation.eulerAngles;
        if (flap2 != null) startRotationVec2 = flap2.transform.localRotation.eulerAngles;
        if (flap3 != null) startRotationVec3 = flap3.transform.localRotation.eulerAngles;
        if (flap4 != null) startRotationVec4 = flap4.transform.localRotation.eulerAngles;

        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    private void OnDestroy()
    {
        if (S_GameUserData.instance != null)
        {
            S_GameUserData.instance.OnLanguageChanged -= UpdateInteractText;
        }
    }


    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform) //& Ouverture du carton
    {
        Open(playerTransform.position);
        UpdateInteractText();
    }

    public string getInteractText() => interactText;
    public Transform getTransform() => transform;

    //! --------------- Fonctions protégées ---------------

    private void Open(Vector3 playerPosition) //& Gére le coroutine d'ouverture
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(DoRotationOpen());
 
        // Son d'ouverture
        //S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.doorOpening, transform.position);
    }

    private IEnumerator DoRotationOpen() //& Ouverture carton (4 volets)
    {
        // Préparer les rotations LOCALES de départ
        Quaternion startRot1 = flap1 != null ? flap1.transform.localRotation : Quaternion.identity;
        Quaternion startRot2 = flap2 != null ? flap2.transform.localRotation : Quaternion.identity;
        Quaternion startRot3 = flap3 != null ? flap3.transform.localRotation : Quaternion.identity;
        Quaternion startRot4 = flap4 != null ? flap4.transform.localRotation : Quaternion.identity;
        
        // Préparer les rotations finales (LOCALES)
        Quaternion endRot1 = flap1 != null ? Quaternion.Euler(startRotationVec1.x + rotationAmount, startRotationVec1.y, startRotationVec1.z) : Quaternion.identity;
        Quaternion endRot2 = flap2 != null ? Quaternion.Euler(startRotationVec2.x + rotationAmount, startRotationVec2.y, startRotationVec2.z) : Quaternion.identity;
        Quaternion endRot3 = flap3 != null ? Quaternion.Euler(startRotationVec3.x - rotationAmount, startRotationVec3.y, startRotationVec3.z) : Quaternion.identity;
        Quaternion endRot4 = flap4 != null ? Quaternion.Euler(startRotationVec4.x - rotationAmount, startRotationVec4.y, startRotationVec4.z) : Quaternion.identity;
        
        float time = 0;
        while (time < 1)
        {
            // Rotation LOCALE de chaque volet (indépendante du parent)
            if (flap1 != null) flap1.transform.localRotation = Quaternion.Slerp(startRot1, endRot1, time);
            if (flap2 != null) flap2.transform.localRotation = Quaternion.Slerp(startRot2, endRot2, time);
            if (flap3 != null) flap3.transform.localRotation = Quaternion.Slerp(startRot3, endRot3, time);
            if (flap4 != null) flap4.transform.localRotation = Quaternion.Slerp(startRot4, endRot4, time);
            
            yield return null;
            time += Time.deltaTime * speed;
        }

        Destroy(this); // Supprime le script de l'objet
    }

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
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
}
