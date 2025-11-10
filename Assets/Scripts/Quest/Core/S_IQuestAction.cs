//! Représente une action/récompense exécutable quand une quête est complétée (donner XP, item, etc.). 

using UnityEngine;

/**
 * Interface simple pour représenter une action/récompense de quête.
 * Implémentations concrètes peuvent être des ScriptableObjects ou des classes utilitaires.
 *
 * @var		public	interfac
 * @global
 */
public interface S_IQuestAction
{
    /// Applique l'action de récompense pour l'instance donnée (par ex. donner item, XP).
    /// <param name="questInstance">Instance de la quête complétée.</param>
    void Execute(S_QuestInstance questInstance);
}

