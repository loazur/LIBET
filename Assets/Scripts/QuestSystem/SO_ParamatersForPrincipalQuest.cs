using UnityEngine;

class SO_ParamatersForPrincipalQuest : ScriptableObject
{
    public int DecalageBetweenQuestDays = 3; // Par défaut 3 jours pour une semaine

    [Tooltip("Fonctionne comme une file, premier entré, premier sorti")]
    public SO_QuestInfo QuestPrincipal;
}