
Info du système de quêtes:

- S_QuestManager doit charger TOUTES les quêtes au démarrage (Resources/Quest/)
- S_LaunchRandomQuest ne référence que les quêtes répétitives journalières
- Les quêtes story peuvent être lancées manuellement via S_QuestPoint
- Ordre des scripts dans la scène n'a pas d'importance (singletons)
- Le menu des quêtes s'ouvre avec la touche définie dans S_UserInput (par défaut "i")

Comment fonctionne une quete:
1. Une quete est définis avec un ScriptableObject (S_QuestData)
    * Une quete contient un titre, une description, un type, des objectifs, des recompenses etc.
2. Une quete est lancée via S_QuestManager.LaunchQuest(questData)
3. S_QuestManager crée une instance de S_Quest en utilisant les données de S_QuestData
4. S_QuestManager suit la progression de la quete et met à jour les objectifs
5. Lorsque tous les objectifs sont complétés, la quete est marquée comme terminée
6. Les recompenses sont distribuées au joueur
7. si la quete est répétitive, elle peut être relancée après un délai défini