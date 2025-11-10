// ! Représente une instance de quête en runtime. Ne pas confondre avec la définition (SO)

// Assets/Scripts/Quest/Core/S_QuestInstance.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Instance runtime d'une quête.
// Stocke la progression des objectifs (par id) et expose TryApplyEvent pour recevoir les QuestEventData.
[Serializable]
public class S_QuestInstance
{
    public string questId;
    public int currentStageIndex;
    public bool IsCompleted;
    public long lastCompletedTimestampUnix;

    // progress: clé = objectiveId, valeur = progress number (par ex. count atteint)
    public Dictionary<string, float> objectiveProgress = new Dictionary<string, float>();

    // ---- constructors / factories ----
    public static S_QuestInstance CreateFromDefinition(S_QuestDefinition def)
    {
        if (def == null) return null;
        var inst = new S_QuestInstance()
        {
            questId = def.questId,
            currentStageIndex = 0,
            IsCompleted = false,
            lastCompletedTimestampUnix = 0,
            objectiveProgress = new Dictionary<string, float>()
        };

        // Initialize objectives progress to 0 for the first stage if possible
        var stage = def.GetStage(0);
        if (stage != null)
        {
            foreach (var obj in stage.objectives)
            {
                if (obj == null) continue;
                if (!string.IsNullOrEmpty(obj.objectiveId))
                    inst.objectiveProgress[obj.objectiveId] = 0f;
            }
        }

        return inst;
    }

    /// <summary>
    /// Recrée depuis les données de sauvegarde.
    /// </summary>
    public static S_QuestInstance FromSaveData(SaveData sd)
    {
        if (sd == null) return null;
        var inst = new S_QuestInstance()
        {
            questId = sd.questId,
            currentStageIndex = sd.currentStageIndex,
            IsCompleted = sd.isCompleted,
            lastCompletedTimestampUnix = sd.lastCompletedUnix,
            objectiveProgress = sd.objectiveProgress != null
                ? new Dictionary<string, float>(sd.objectiveProgress)
                : new Dictionary<string, float>()
        };
        return inst;
    }

    /// <summary>
    /// Sérialisation compacte pour sauvegarde.
    /// </summary>
    public SaveData ToSaveData()
    {
        return new SaveData
        {
            questId = questId,
            currentStageIndex = currentStageIndex,
            isCompleted = IsCompleted,
            lastCompletedUnix = lastCompletedTimestampUnix,
            objectiveProgress = new Dictionary<string, float>(objectiveProgress)
        };
    }

    // ---- progression / events ----

    /// <summary>
    /// Reçoit un event global et tente de l'appliquer aux objectifs du stage courant.
    /// Retourne true si la progression de la quête a changé (progress ou complétion).
    /// </summary>
    public bool TryApplyEvent(QuestEventData ev)
    {
        var def = S_QuestDatabase.Instance?.GetDefinition(questId);
        if (def == null) return false;

        if (IsCompleted) return false;

        var stage = def.GetStage(currentStageIndex);
        if (stage == null) return false;

        bool changed = false;

        // Iterate through objectives of this stage
        foreach (var objDef in stage.objectives)
        {
            if (objDef == null) continue;
            // Each objective definition must implement a method to evaluate an event.
            // We call a generic EvaluateEvent(objDef, ev) - objective-specific logic lives there.
            bool objectiveChanged = ObjectiveEvaluator.EvaluateAndApply(objDef, ev, this);
            if (objectiveChanged) changed = true;
        }

        // If all objectives of the stage are completed -> advance
        if (AllObjectivesComplete(stage))
        {
            AdvanceStage(def);
            changed = true;
        }

        return changed;
    }

    private bool AllObjectivesComplete(S_QuestStage stage)
    {
        foreach (var obj in stage.objectives)
        {
            if (obj == null) continue;
            // Each objective has an objectiveId; we consider complete if progress >= requiredCount
            float prog = 0f;
            objectiveProgress.TryGetValue(obj.objectiveId, out prog);
            if (!obj.IsSatisfied(prog)) return false;
        }
        return true;
    }

    private void AdvanceStage(S_QuestDefinition def)
    {
        currentStageIndex++;
        var nextStage = def.GetStage(currentStageIndex);
        if (nextStage == null)
        {
            // No more stages -> quest complete
            IsCompleted = true;
            lastCompletedTimestampUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        else
        {
            // Initialize progress entries for the new stage
            foreach (var obj in nextStage.objectives)
            {
                if (obj == null) continue;
                if (!objectiveProgress.ContainsKey(obj.objectiveId))
                    objectiveProgress[obj.objectiveId] = 0f;
            }
        }
    }

    /// <summary>
    /// Appliquer une progression manuelle (utile pour tests)
    /// </summary>
    public void AddProgress(string objectiveId, float amount)
    {
        if (!objectiveProgress.ContainsKey(objectiveId))
            objectiveProgress[objectiveId] = 0f;
        objectiveProgress[objectiveId] += amount;
    }

    // ---- Save Data container ----
    [Serializable]
    public class SaveData
    {
        public string questId;
        public int currentStageIndex;
        public bool isCompleted;
        public long lastCompletedUnix;
        public Dictionary<string, float> objectiveProgress;
    }
}


/// Helper statique : adapte un ObjectiveDefinition + event -> application sur S_QuestInstance.
/// Le but est de garder TryApplyEvent propre ; la logique spécifique aux différents objectifs
/// (interact, place at, sit) est implémentée dans ObjectiveEvaluator.
public static class ObjectiveEvaluator
{
    /// <summary>
    /// Retourne true si la progression a changé pour l'instance.
    /// </summary>
    public static bool EvaluateAndApply(S_ObjectiveDefinition objDef, QuestEventData ev, S_QuestInstance instance)
    {
        if (objDef == null || ev == null || instance == null) return false;

        // Exemple simple : objective type "Interact" attend eventType "Interact" and objectId equal
        // The S_ObjectiveDefinition should expose objectiveType, targetId, requiredCount, etc.

        // We keep logic data-driven: inspect objDef.objectiveType and parameters
        switch (objDef.objectiveType)
        {
            case S_ObjectiveType.Interact:
                if (ev.eventType == QuestEventType.Interact && ev.objectId == objDef.targetId)
                {
                    // increment count by 1
                    var prev = instance.objectiveProgress.ContainsKey(objDef.objectiveId) ? instance.objectiveProgress[objDef.objectiveId] : 0f;
                    instance.objectiveProgress[objDef.objectiveId] = prev + 1f;
                    return true;
                }
                break;

            case S_ObjectiveType.PlaceAt:
                if (ev.eventType == QuestEventType.Place && ev.itemId == objDef.itemId)
                {
                    // Place into allowed zone?
                    if (objDef.allowedZoneIds == null || objDef.allowedZoneIds.Length == 0 || Array.Exists(objDef.allowedZoneIds, z => z == ev.zoneId))
                    {
                        instance.objectiveProgress[objDef.objectiveId] = 1f;
                        return true;
                    }
                }
                break;

            case S_ObjectiveType.SitAtPoint:
                if (ev.eventType == QuestEventType.Sit && ev.seatId == objDef.targetId)
                {
                    if (ev.duration >= objDef.requiredDuration)
                    {
                        instance.objectiveProgress[objDef.objectiveId] = 1f;
                        return true;
                    }
                }
                break;

            case S_ObjectiveType.Custom:
                // If objective has a custom condition component (ScriptableObject implementing S_IQuestCondition)
                if (objDef.customCondition != null)
                {
                    bool res = objDef.customCondition.Evaluate(ev);
                    if (res)
                    {
                        instance.objectiveProgress[objDef.objectiveId] = 1f;
                        return true;
                    }
                }
                break;
        }

        return false;
    }
}
