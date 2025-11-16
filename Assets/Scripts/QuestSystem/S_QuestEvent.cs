using System;

public class S_QuestEvent 
{
   public event Action<string> OnStartQuest;
   public event Action<string> OnAdvanceQuest;
   public event Action<string> OnFinishQuest;
   public event Action<S_Quest> onQuestStateChange;

   public void StartQuest(string questID)
   {
       if (OnStartQuest != null)
       {
           OnStartQuest(questID);
       }
   }

   public void AdvanceQuest(string questID)
   {
       if (OnAdvanceQuest != null)
       {
           OnAdvanceQuest(questID);
       }
   }

   public void FinishQuest(string questID)
   {
       if (OnFinishQuest != null)
       {
           OnFinishQuest(questID);
       }
   }

   public void QuestStateChange(S_Quest quest)
   {
       if (onQuestStateChange != null)
       {
           onQuestStateChange(quest);
       }
   }
}
