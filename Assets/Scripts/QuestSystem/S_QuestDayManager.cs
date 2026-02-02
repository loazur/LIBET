
using UnityEngine;


/**
 * Gère les éléments de quêtes spécifiques à chaque jour (prefabs, objets débloqués par jour, etc.)
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Monday, February 2nd, 2026.
 * @global
 */
public class S_QuestDayManager : MonoBehaviour
{
    public static S_QuestDayManager instance { get; private set; }

    [Header("Prefabs spécifiques aux quêtes")]
    [SerializeField] private GameObject keyOnDoorPrefab; // Prefab de la clé sur porte (jour 2)
    [SerializeField] private GameObject notePrefab;

    private S_TakeKey keyUnderDoor;

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
        // Initialiser la référence à S_TakeKey
        if (keyOnDoorPrefab != null)
        {
            keyUnderDoor = keyOnDoorPrefab.GetComponent<S_TakeKey>();
        }

        // Désactiver les prefabs au début
        DisableQuestPrefabs();

        // S'abonner aux événements du système de jours
        if (S_DaysManager.instance != null)
        {
            S_DaysManager.instance.OnDayEnd += OnDayEnd;
        }
        else
        {
            Debug.LogWarning("[S_QuestDayManager] S_DaysManager.instance est NULL dans Start!");
        }
    }

    void OnDestroy()
    {
        // Se désabonner des événements
        if (S_DaysManager.instance != null)
        {
            S_DaysManager.instance.OnDayEnd -= OnDayEnd;
        }
    }

    /**
     * Désactive tous les prefabs de quêtes
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, February 2nd, 2026.
     * @access	public
     * @return	void
     */
    public void DisableQuestPrefabs()
    {
        if (keyOnDoorPrefab != null)
            keyOnDoorPrefab.SetActive(false);
        
        if (notePrefab != null)
            notePrefab.SetActive(false);
    }

    /**
     * Met à jour l'état des prefabs en fonction du jour actuel
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, February 2nd, 2026.
     * @access	public
     * @param	int	currentDay	
     * @return	void
     */
    public void UpdateQuestPrefabsForDay(int currentDay)
    {
        // Activer les prefabs à partir du jour 2
        if (currentDay >= 2)
        {
            ActivateDay2Prefabs();
        }
        else
        {
            DisableQuestPrefabs();
        }
    }

    /**
     * Met à jour l'état des prefabs lors d'un restart de jour
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, February 2nd, 2026.
     * @access	public
     * @param	int	currentDay	
     * @return	void
     */
    public void UpdateQuestPrefabsOnRestart(int currentDay)
    {
        // Activer les prefabs si jour >= 2 ET que la clé n'a pas été prise
        if (currentDay >= 2 && keyUnderDoor != null && !keyUnderDoor.isKeyTaken)
        {
            ActivateDay2Prefabs();
            Debug.Log($"[S_QuestDayManager] KeyOnDoorPrefab activé pour le jour {currentDay}");
        }
    }

    /**
     * Active les prefabs du jour 2 (clé sous la porte et note)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, February 2nd, 2026.
     * @access	private
     * @return	void
     */
    private void ActivateDay2Prefabs()
    {
        if (keyOnDoorPrefab != null)
            keyOnDoorPrefab.SetActive(true);
        
        if (notePrefab != null)
            notePrefab.SetActive(true);
    }

    /**
     * Vérifie si la clé sous la porte a été prise
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, February 2nd, 2026.
     * @access	public
     * @return	mixed
     */
    public bool IsKeyTaken()
    {
        return keyUnderDoor != null && keyUnderDoor.isKeyTaken;
    }

    private void OnDayEnd()
    {
        // Logique à exécuter à la fin d'un jour si nécessaire
    }
}