// using UnityEngine;
// using UnityEditor;

// [CustomEditor(typeof(S_QuestManager))]
// public class QuestManagerEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();
        
//         S_QuestManager questManager = (S_QuestManager)target;
        
//         EditorGUILayout.Space(10);
//         EditorGUILayout.LabelField("Development Tools", EditorStyles.boldLabel);
        
//         if (GUILayout.Button("Reset All Quest Progress"))
//         {
//             if (EditorUtility.DisplayDialog("Reset Quest Progress", 
//                 "Are you sure you want to delete all saved quest progress? This cannot be undone.", 
//                 "Yes, Reset All", 
//                 "Cancel"))
//             {
//                 ResetAllQuestProgress();
//             }
//         }
        
//         if (GUILayout.Button("List All Saved Quests"))
//         {
//             ListAllSavedQuests();
//         }
//     }
    
//     private void ResetAllQuestProgress()
//     {
//         SO_QuestInfo[] allQuests = Resources.LoadAll<SO_QuestInfo>("Quest");
        
//         int resetCount = 0;
//         foreach (SO_QuestInfo questInfo in allQuests)
//         {
//             string key = "Quest_" + questInfo.id;
//             if (PlayerPrefs.HasKey(key))
//             {
//                 PlayerPrefs.DeleteKey(key);
//                 resetCount++;
//                 Debug.Log($"Deleted quest save: {questInfo.displayName} (ID: {questInfo.id})");
//             }
//         }
        
//         PlayerPrefs.Save();
//         Debug.Log($"<color=green>Reset {resetCount} quest(s) successfully!</color>");
//         EditorUtility.DisplayDialog("Success", $"Reset {resetCount} quest(s) successfully!", "OK");
//     }
    
//     private void ListAllSavedQuests()
//     {
//         SO_QuestInfo[] allQuests = Resources.LoadAll<SO_QuestInfo>("Quest");
        
//         Debug.Log("=== SAVED QUESTS ===");
//         int savedCount = 0;
        
//         foreach (SO_QuestInfo questInfo in allQuests)
//         {
//             string key = "Quest_" + questInfo.id;
//             if (PlayerPrefs.HasKey(key))
//             {
//                 string data = PlayerPrefs.GetString(key);
//                 S_QuestData questData = JsonUtility.FromJson<S_QuestData>(data);
//                 Debug.Log($"'{questInfo.displayName}' - State: {questData.state}, Step: {questData.index}");
//                 savedCount++;
//             }
//         }
        
//         if (savedCount == 0)
//         {
//             Debug.Log("No saved quests found.");
//         }
//         else
//         {
//             Debug.Log($"=== Total: {savedCount} saved quest(s) ===");
//         }
//     }
// }
