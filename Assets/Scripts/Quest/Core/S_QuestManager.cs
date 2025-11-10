using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IS_QuestManager
{
    static abstract S_QuestManager Instance { get; }

    void ClearAllProgress();
    void LoadAllQuests();
    void OnEventTriggered(QuestEventData ev);
    void SaveAllQuests();
    bool StartQuest(string questId);
}

/**
 * Singleton manager pour les quêtes
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Monday, November 10th, 2025.
 * @global
 */
[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class S_QuestManager : MonoBehaviour, IS_QuestManager
{

    // * ==========================================================================================
    public static S_QuestManager Instance { get; private set; }

    [Header("Quêtes actives en runtime (progression)")]
    public List<S_QuestInstance> activeQuests = new List<S_QuestInstance>();

    [Header("Quêtes complétées (simple cache en runtime, sauvegarder/restaurer via Save/Load)")]
    public HashSet<string> completedQuestIds = new HashSet<string>();

    [Header("Nome de la clé PlayerPrefs pour la sauvegarde")]
    [SerializeField] private string playerPrefsKey = "QuestSystem_Save_v1";
    // * ==========================================================================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllQuests();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    # region TODO :  réparer ici
    /**
     * Démarre une quête si possible
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @access	public
     * @param	string	questId	
     * @return	boolean
     */
    public bool StartQuest(string questId)
    {
        if (string.IsNullOrEmpty(questId))
        {
            Debug.LogError("[QuestManager] StartQuest appelé avec un id vide.");
            return false;
        }

        // Si déjà active -> on ne la démarre pas à nouveau (sauf si on veut permettre les duplicates)
        if (activeQuests.Any(q => q.questId == questId && !q.IsCompleted))
        {
            Debug.Log($"[QuestManager] Quest {questId} est déjà active.");
            return false;
        }

        // Si déjà complétée et la quest n'est pas repeatable -> on refuse
        var def = S_QuestDatabase.Instance.GetDefinition(questId);
        if (def == null)
        {
            Debug.LogError($"[QuestManager] QuestDefinition introuvable pour id = {questId}");
            return false;
        }

        if (!def.isRepeatable && completedQuestIds.Contains(questId))
        {
            Debug.Log($"[QuestManager] Quest {questId} a déjà été complétée et n'est pas repeatable.");
            return false;
        }

        // Crée une instance à partir de la définition — implémenter CreateFromDefinition sur S_QuestInstance
        S_QuestInstance instance = S_QuestInstance.CreateFromDefinition(def);
        if (instance == null)
        {
            Debug.LogError("[QuestManager] Impossible de créer l'instance de quête.");
            return false;
        }

        activeQuests.Add(instance);
        OnQuestStarted(instance);
        SaveAllQuests(); // sauvegarde automatique
        Debug.Log($"[QuestManager] Quest {questId} démarrée.");
        return true;
    }

    # region TODO :  réparer ici
    /**
     * Appelé par le système d'interactions / events. Passe EventData aux quêtes actives.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @access	public
     * @param	questeventdata	ev	
     * @return	void
     */
    public void OnEventTriggered(QuestEventData ev)
    {
        if (ev == null) return;

        bool anyProgress = false;

        // Itérer en copie pour permettre suppression durant l'itération
        var list = activeQuests.ToList();
        foreach (var qi in list)
        {
            if (qi.IsCompleted) continue;

            // TryApplyEvent retourne true si la quête a changé d'état (progress/complete)
            bool progressed = false;
            try
            {
                progressed = qi.TryApplyEvent(ev);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[QuestManager] Erreur lors de l'application d'un event sur la quest {qi.questId}: {ex}");
            }

            if (progressed)
            {
                anyProgress = true;
                OnQuestUpdated(qi);

                if (qi.IsCompleted)
                {
                    OnQuestCompleted(qi);
                    // si la quête est complétée, la sortir des actives (mais garder en historique)
                    activeQuests.Remove(qi);
                    completedQuestIds.Add(qi.questId);
                }
            }
        }

        if (anyProgress)
        {
            SaveAllQuests();
        }
    }

    #region Save / Load (simple JSON via PlayerPrefs)
    /**
     * Note : pour un vrai projet préfèrer un fichier JSON sur disque ou un système de save central (Addressables, etc.)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @access	public
     * @return	void
     */
    public void SaveAllQuests()
    {
        try
        {
            var container = new QuestSaveContainer()
            {
                completed = completedQuestIds.ToList(),
                active = activeQuests.Select(q => q.ToSaveData()).ToList()
            };
            string json = JsonUtility.ToJson(container);
            PlayerPrefs.SetString(playerPrefsKey, json);
            PlayerPrefs.Save();
#if UNITY_EDITOR
            Debug.Log("[QuestManager] Sauvegarde effectuée.");
#endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"[QuestManager] Erreur lors de la sauvegarde : {ex}");
        }
    }

    # region TODO :  réparer ici
    /**
     * Charge toutes les quêtes sauvegardées
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @access	public
     * @return	void
     */
    public void LoadAllQuests()
    {
        try
        {
            if (!PlayerPrefs.HasKey(playerPrefsKey))
            {
                // Rien à charger
                return;
            }

            string json = PlayerPrefs.GetString(playerPrefsKey);
            if (string.IsNullOrEmpty(json)) return;

            var container = JsonUtility.FromJson<QuestSaveContainer>(json);
            if (container == null) return;

            completedQuestIds = new HashSet<string>(container.completed ?? new List<string>());

            activeQuests.Clear();
            if (container.active != null)
            {
                foreach (var saveData in container.active)
                {
                    // S_QuestInstance doit exposer une factory pour recréer depuis saveData
                    var inst = S_QuestInstance.FromSaveData(saveData);
                    if (inst != null)
                        activeQuests.Add(inst);
                }
            }
#if UNITY_EDITOR
            Debug.Log("[QuestManager] Chargement des quêtes terminé.");
#endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"[QuestManager] Erreur lors du chargement : {ex}");
        }
    }

    # region TODO :  réparer ici
    // Container minimal pour sérialisation
    [Serializable]
    private class QuestSaveContainer
    {
        public List<string> completed;
        public List<S_QuestInstance.SaveData> active;
    }
    #endregion

    #region Hooks / Events (peuvent être remplacés par des UnityEvents)
    /**
     * appelé quand une quête démarre
     *
     * @var		virtual	voi
     */
    protected virtual void OnQuestStarted(S_QuestInstance instance)
    {
        // TODO : envoyer event UI, jouer son, etc.
        // Ex : UI_QuestLog.Instance.Add(instance);
    }

    # region TODO : FINIR ICI
    /**
     * appelé quand une quête fait une progression
     *
     * @var		virtual	voi
     */
    protected virtual void OnQuestUpdated(S_QuestInstance instance)
    {
        // TODO : notifier UI
    }
    #endregion

    # region TODO : FINIR ICI
    /**
     * appelé quand une quête se termine
     *
     * @var		virtual	voi
     */
    protected virtual void OnQuestCompleted(S_QuestInstance instance)
    {
        Debug.Log($"[QuestManager] Quest {instance.questId} complétée.");
        // TODO : appliquer récompenses, notifier UI, triggers (ex: faction change)
        // Exemple : RewardApplier.Apply(instance.GetDefinition().rewards);
    }
    #endregion

    /**
     * Méthode utilitaire publique pour nettoyer l'état (éditeur / debug)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @access	public
     * @return	void
     */
    public void ClearAllProgress()
    {
        activeQuests.Clear();
        completedQuestIds.Clear();
        PlayerPrefs.DeleteKey(playerPrefsKey);
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
