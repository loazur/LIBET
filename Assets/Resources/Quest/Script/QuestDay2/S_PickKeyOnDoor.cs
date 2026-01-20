


using UnityEngine;
using FMODUnity;
using System.Collections;

/**
 * Script pour jouer un son de clé jusqu'à ce qu'elle soit ramassée.
 * Fonctionne comme le système du piano - son en boucle qui s'arrête à la collecte.
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Monday, January 19th, 2026.
 * @global
 */
public class S_PickKeyOnDoor : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private EventReference AlarmSound; // Son FMOD à jouer en boucle
    [SerializeField] private float soundLoopInterval = 2f; // Intervalle entre chaque son (en secondes)
    
    [Header("Key Configuration")]
    [SerializeField] private string doorID = "door_01"; // ID de la porte (doit correspondre à S_TakeKey)
    [SerializeField] private string keyID = "key_01"; // ID de la clé (doit correspondre à S_TakeKey)

    [Header("Detection")]
    [SerializeField] private float maxDistance = 10f; // Distance maximale pour jouer le son

    private StudioEventEmitter soundEmitter;
    private Transform playerTransform;
    private bool isCollected = false;
    private Coroutine soundLoopRoutine;
    private S_TakeKey keyScript;

    //*========================================================

    void Start()
    {
        // Récupérer le script S_TakeKey sur cet objet
        keyScript = GetComponent<S_TakeKey>();
        if (keyScript == null)
        {
            Debug.LogWarning("[S_PickKeyOnDoor] Aucun S_TakeKey trouvé sur cet objet!");
            enabled = false;
            return;
        }

        // Initialiser l'émetteur audio
        soundEmitter = S_AudioManager.instance.InitializeEventEmitter(AlarmSound, gameObject);
        if (soundEmitter != null)
        {
            soundEmitter.Stop();
        }

        // Attendre que le joueur soit prêt
        StartCoroutine(InitializeWhenReady());
    }

    void OnDestroy()
    {
        // S'assurer d'arrêter le son
        if (soundEmitter != null && soundEmitter.IsPlaying())
        {
            soundEmitter.Stop();
        }

        // Arrêter la coroutine
        if (soundLoopRoutine != null)
        {
            StopCoroutine(soundLoopRoutine);
        }
    }

    private IEnumerator InitializeWhenReady()
    {
        // Attendre que le GameManager soit prêt
        while (S_GameManager.instance == null || S_GameManager.instance.playerEvents == null)
        {
            yield return null;
        }

        // S'abonner à l'événement de collecte de clé
        S_GameManager.instance.playerEvents.onKeyCollected += OnKeyCollected;

        // Trouver le joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            // Démarrer la boucle de son
            soundLoopRoutine = StartCoroutine(SoundLoop());
        }
        else
        {
            Debug.LogWarning("[S_PickKeyOnDoor] Impossible de trouver le joueur!");
        }
    }

    /**
     * Coroutine qui joue le son en boucle tant que la clé n'est pas collectée
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 19th, 2026.
     * @access	private
     * @return	IEnumerator
     */
    private IEnumerator SoundLoop()
    {
        while (!isCollected)
        {
            // Vérifier la distance avec le joueur
            if (playerTransform != null)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);

                if (distance <= maxDistance)
                {
                    // Le joueur est assez proche, jouer le son
                    if (soundEmitter != null && !soundEmitter.IsPlaying())
                    {
                        soundEmitter.Play();
                    }
                }
                else
                {
                    // Le joueur est trop loin, arrêter le son
                    if (soundEmitter != null && soundEmitter.IsPlaying())
                    {
                        soundEmitter.Stop();
                    }
                }
            }

            yield return new WaitForSeconds(soundLoopInterval);
        }

        // Arrêter le son quand la boucle se termine
        if (soundEmitter != null && soundEmitter.IsPlaying())
        {
            soundEmitter.Stop();
        }
    }

    /**
     * Callback appelé quand une clé est collectée
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 19th, 2026.
     * @access	private
     * @param	GameObject	key   
     * @param	string    	collectedDoorID
     * @param	string    	collectedKeyID
     * @return	void
     */
    private void OnKeyCollected(GameObject key, string collectedDoorID, string collectedKeyID)
    {
        // Vérifier si c'est cette clé qui a été collectée
        if (collectedDoorID == doorID && collectedKeyID == keyID)
        {
            Debug.Log($"[S_PickKeyOnDoor] Clé {keyID} collectée - arrêt du son");
            isCollected = true;

            // Arrêter le son immédiatement
            if (soundEmitter != null && soundEmitter.IsPlaying())
            {
                soundEmitter.Stop();
            }

            // Arrêter la coroutine
            if (soundLoopRoutine != null)
            {
                StopCoroutine(soundLoopRoutine);
            }

            // Se désabonner de l'événement
            if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
            {
                S_GameManager.instance.playerEvents.onKeyCollected -= OnKeyCollected;
            }
        }
    }
}