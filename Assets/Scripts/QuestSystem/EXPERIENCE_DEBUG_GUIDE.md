# 🎮 Guide de Diagnostic - Système d'Expérience

## 📊 Comment Fonctionne le Système

### Configuration

**Dans `S_GlobalConstants` :**
```csharp
experienceToLevelUp = 100
```
Il faut **100 points d'expérience** pour monter d'un niveau.

**Dans `SO_QuestInfo` :**
```csharp
experienceReward = X  // Points d'XP donnés à la fin de la quête
```

---

## 🔍 Logs de Diagnostic

### Quand une quête se termine, vous devriez voir :

```
🔵 [QuestManager] Distribution des récompenses pour la quête: MaQuete
🔵 [QuestManager] Distribution de 50 points d'expérience
🟢 [QuestManager] Événement ExperienceGained déclenché avec 50 XP

🔵 [PlayerLevelManager] Réception de 50 XP | Niveau actuel: 1 | XP actuel: 0/100
🔵 [PlayerLevelManager] Après ajout: 50/100 XP
🟡 [PlayerLevelManager] Pas assez d'XP pour monter de niveau (besoin de 50 XP supplémentaires)
```

### Si le joueur monte de niveau :

```
🟢 [PlayerLevelManager] NIVEAU SUPÉRIEUR ! Nouveau niveau: 2 | XP restant: 25
```

---

## 🚨 Diagnostic des Problèmes

### ❌ Problème 1 : "Aucune expérience configurée"

**Log :**
```
[QuestManager] Aucune expérience configurée (experienceReward = 0)
```

**Solution :**
1. Ouvrir le `SO_QuestInfo` de votre quête
2. Section "Experience Reward"
3. Mettre `experienceReward` à une valeur > 0 (ex: 50, 100, etc.)

---

### ❌ Problème 2 : "GameManager ou PlayerEvents est null"

**Log :**
```
[QuestManager] GameManager ou PlayerEvents est null ! Impossible de donner l'expérience.
```

**Solution :**
- Vérifier que `S_GameManager` est présent dans la scène
- Vérifier qu'il est actif

---

### ❌ Problème 3 : "Pas de log de PlayerLevelManager"

**Symptôme :** Vous voyez les logs de QuestManager mais PAS ceux de PlayerLevelManager

**Causes possibles :**

1. **Le S_PlayerLevelManager n'est pas dans la scène**
   - Vérifier dans la hiérarchie
   - Le composant doit être actif

2. **L'événement n'est pas écouté**
   - Vérifier que `S_PlayerLevelManager` s'est bien abonné à `onExperienceGained`
   - Regarder les logs au démarrage du jeu

---

### ❌ Problème 4 : "L'XP est donnée mais le niveau ne change pas"

**Log :**
```
[PlayerLevelManager] Réception de 25 XP | Niveau actuel: 1 | XP actuel: 10/100
[PlayerLevelManager] Après ajout: 35/100 XP
[PlayerLevelManager] Pas assez d'XP pour monter de niveau (besoin de 65 XP supplémentaires)
```

**Explication :**
C'est **NORMAL** ! Il faut accumuler **100 XP** pour passer au niveau 2.

**Solution :**
- Augmenter `experienceReward` dans vos quêtes (ex: 100 au lieu de 25)
- OU compléter plusieurs quêtes pour atteindre 100 XP

---

## 📋 Exemples de Configuration

### Quête Facile (petit bonus XP)
```
experienceReward = 25
→ Il faudra 4 quêtes pour monter au niveau 2
```

### Quête Moyenne
```
experienceReward = 50
→ Il faudra 2 quêtes pour monter au niveau 2
```

### Quête Importante
```
experienceReward = 100
→ Monte directement au niveau 2
```

### Quête Épique
```
experienceReward = 250
→ Monte au niveau 3 avec 50 XP restants
```

---

## 🎯 Checklist de Vérification

Avant de terminer une quête, vérifiez :

- [ ] `SO_QuestInfo` a un `experienceReward` > 0
- [ ] `S_GameManager` est présent et actif dans la scène
- [ ] `S_PlayerLevelManager` est présent et actif dans la scène
- [ ] Les logs de QuestManager apparaissent (événement déclenché)
- [ ] Les logs de PlayerLevelManager apparaissent (événement reçu)

---

## 🔧 Test Manuel

Pour tester rapidement le système d'expérience :

1. **Configurer une quête de test**
   ```
   experienceReward = 100
   ```

2. **Compléter la quête**

3. **Vérifier les logs** - vous devriez voir :
   ```
   [PlayerLevelManager] NIVEAU SUPÉRIEUR ! Nouveau niveau: 2
   ```

---

## 📊 Comprendre l'Accumulation d'XP

```
Niveau 1 → Niveau 2 : 100 XP nécessaires
Niveau 2 → Niveau 3 : 100 XP nécessaires
Niveau 3 → Niveau 4 : 100 XP nécessaires
etc.
```

Si vous voulez une progression différente, modifiez `S_GlobalConstants.experienceToLevelUp`.

---

**Version** : v1.0  
**Date** : 8 janvier 2026
