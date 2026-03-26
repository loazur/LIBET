
using UnityEngine;


/**
 * Gère les éléments de quêtes spécifiques à chaque jour (prefabs, objets débloqués par jour, etc.)
 * Les prefabs sont instanciés dynamiquement au lieu d'être activés/désactivés.
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v2.0.0	Friday, February 28th, 2026.
 * @global
 */
public class S_QuestDayManager : MonoBehaviour
{
    public static S_QuestDayManager instance { get; private set; }

    [Header("Prefabs à instancier")]
    [SerializeField] private GameObject keyOnDoorPrefab;
    [SerializeField] private GameObject notePrefab;

    [Header("Points de spawn")]
    [SerializeField] private Transform keySpawnPoint;
    [SerializeField] private Transform noteSpawnPoint;

    // Instances vivantes dans la scène
    private GameObject keyInstance;
    private GameObject noteInstance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (S_DaysManager.instance != null)
        {
            S_DaysManager.instance.OnDayEnd += OnDayEnd;

            // Synchroniser immédiatement l'état des prefabs avec le jour actuel.
            // Cela couvre le premier chargement de scène et les parties chargées en cours de journée.
            SyncQuestPrefabsWithCurrentDay();
        }
        else
        {
            Debug.LogWarning("[S_QuestDayManager] S_DaysManager.instance est NULL dans Start!");
        }
    }

    void OnDestroy()
    {
        if (S_DaysManager.instance != null)
        {
            S_DaysManager.instance.OnDayEnd -= OnDayEnd;
        }
    }

    /**
     * Met à jour l'état des prefabs en fonction du jour actuel.
     * Instancie les objets au jour 2+, les détruit sinon.
     */
    public void UpdateQuestPrefabsForDay(int currentDay)
    {
        if (currentDay >= 2)
        {
            SpawnDay2Prefabs();
        }
        else
        {
            DestroyQuestInstances();
        }
    }

    private void SyncQuestPrefabsWithCurrentDay()
    {
        if (S_DaysManager.instance == null)
        {
            return;
        }

        UpdateQuestPrefabsForDay(S_DaysManager.instance.GetCurrentDay());
    }

    /**
     * Met à jour l'état des prefabs lors d'un restart de jour.
     * Réinstancie les objets si le jour >= 2.
     */
    public void UpdateQuestPrefabsOnRestart(int currentDay)
    {
        // Toujours nettoyer les anciennes instances avant de respawn
        DestroyQuestInstances();

        if (currentDay >= 2)
        {
            SpawnDay2Prefabs();
            Debug.Log($"[S_QuestDayManager] Prefabs réinstanciés pour le restart du jour {currentDay}");
        }
    }

    /**
     * Instancie les prefabs du jour 2 (clé sous la porte et note).
     * Ne réinstancie pas si une instance existe déjà.
     */
    private void SpawnDay2Prefabs()
    {
        if (keyInstance == null && keyOnDoorPrefab != null && keySpawnPoint != null)
        {
            keyInstance = Instantiate(keyOnDoorPrefab, keySpawnPoint.position, keySpawnPoint.rotation);
            Debug.Log("[S_QuestDayManager] keyOnDoorPrefab instancié");
        }

        if (noteInstance == null && notePrefab != null && noteSpawnPoint != null)
        {
            noteInstance = Instantiate(notePrefab, noteSpawnPoint.position, noteSpawnPoint.rotation);
            Debug.Log("[S_QuestDayManager] notePrefab instancié");
        }
    }

    /**
     * Détruit les instances en cours des prefabs de quêtes.
     */
    public void DestroyQuestInstances()
    {
        if (keyInstance != null)
        {
            Destroy(keyInstance);
            keyInstance = null;
        }

        if (noteInstance != null)
        {
            Destroy(noteInstance);
            noteInstance = null;
        }
    }

    /**
     * Vérifie si la clé sous la porte a été prise.
     */
    public bool IsKeyTaken()
    {
        if (keyInstance == null) return false;

        S_TakeKey takeKey = keyInstance.GetComponent<S_TakeKey>();
        return takeKey != null && takeKey.isKeyTaken;
    }

    private void OnDayEnd()
    {
        // Nettoyer les instances à la fin du jour
        DestroyQuestInstances();
    }
}