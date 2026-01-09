# 🎯 Configuration QuestPoint - Guide Simplifié

## ⚙️ Les 4 Paramètres

### 📍 Config
- **`startPoint`** : Ce point peut démarrer la quête
- **`finishPoint`** : Ce point peut terminer la quête

### 🎮 Interaction Mode
- **`autoStartQuest`** : Démarre au contact (true) ou avec E (false)
- **`autoFinishQuest`** : Termine au contact (true) ou avec E (false)

---

## 🎯 Comment ça marche ?

### Démarrage de Quête

| startPoint | autoStartQuest | Résultat |
|------------|----------------|----------|
| ✅ true | ✅ true | Quête démarre **automatiquement** au contact |
| ✅ true | ❌ false | Joueur doit **appuyer sur E** pour démarrer |
| ❌ false | N/A | Ce point ne peut pas démarrer la quête |

### Fin de Quête (après avoir terminé toutes les étapes)

| finishPoint | autoFinishQuest | Résultat |
|-------------|-----------------|----------|
| ✅ true | ✅ true | Quête se termine **automatiquement** au contact |
| ✅ true | ❌ false | Joueur doit **appuyer sur E** pour terminer |
| ❌ false | N/A | Ce point ne peut pas terminer la quête |

---

## 📋 Configurations Recommandées

### 1️⃣ Quête Simple (Auto-Start, Auto-Finish)

**QuestPoint de départ :**
```
✅ startPoint = true
❌ finishPoint = false
✅ autoStartQuest = true
```

**QuestPoint de fin :**
```
❌ startPoint = false
✅ finishPoint = true
✅ autoFinishQuest = true
```

**Résultat :**
- Joueur entre dans la zone → Quête démarre
- Joueur termine les étapes → Retourne à la zone → Quête se termine + récompenses

---

### 2️⃣ Quête avec Interaction Manuelle

**QuestPoint de départ :**
```
✅ startPoint = true
❌ finishPoint = false
❌ autoStartQuest = false
```

**QuestPoint de fin :**
```
❌ startPoint = false
✅ finishPoint = true
❌ autoFinishQuest = false
```

**Résultat :**
- Joueur entre dans la zone → Message "Appuyez sur E"
- Joueur appuie sur E → Quête démarre
- Après les étapes → Retourne à la zone → Message "Appuyez sur E"
- Joueur appuie sur E → Quête se termine + récompenses

---

### 3️⃣ Même Point pour Démarrer et Terminer (NPC)

```
✅ startPoint = true
✅ finishPoint = true
✅ autoStartQuest = true
✅ autoFinishQuest = true
```

**Résultat :**
- 1er contact → Quête démarre
- Après les étapes, revenir au même point → Quête se termine

---

## 🔍 Logs de Diagnostic

### Au démarrage de quête :

✅ **Succès (auto)** :
```
[QuestPoint] Démarrage automatique de la quête 'MaQuete'
```

⚠️ **Mode manuel** :
```
[QuestPoint] Quête 'MaQuete' peut démarrer mais AutoStartQuest est désactivé. 
Appuyez sur Submit pour démarrer.
```

### À la fin de quête :

✅ **Succès (auto)** :
```
[QuestPoint] Finalisation automatique de la quête 'MaQuete'
[QuestManager] Terminer la quête: MaQuete
[QuestManager] Distribution des récompenses...
```

⚠️ **Mode manuel** :
```
[QuestPoint] Quête 'MaQuete' peut être terminée mais AutoFinish est désactivé. 
Appuyez sur Submit pour terminer.
```

---

## 🚨 Problèmes Courants

### ❌ "Ma quête ne démarre pas automatiquement"

**Vérifiez dans l'Inspector du QuestPoint :**
```
✅ startPoint = true
✅ autoStartQuest = true  ← Doit être TRUE !
```

---

### ❌ "Ma quête ne se termine pas après les étapes"

**Deux possibilités :**

1. **Vous n'avez pas de QuestPoint de fin**
   - Créez un GameObject avec S_QuestPoint
   - Configurez : `finishPoint = true` + `autoFinishQuest = true`

2. **Le paramètre est mal configuré**
   ```
   ✅ finishPoint = true
   ✅ autoFinishQuest = true  ← Doit être TRUE pour auto !
   ```

---

### ❌ "Les récompenses ne sont pas données"

**Vérifiez les logs dans cet ordre :**

1. La quête atteint-elle `CAN_FINISH` ?
   ```
   [QuestManager] Toutes les étapes de 'X' sont terminées. État: CAN_FINISH
   ```

2. Le joueur entre-t-il dans le QuestPoint de fin ?
   ```
   [QuestPoint] Joueur entre dans le trigger - État: CAN_FINISH
   ```

3. La quête se termine-t-elle ?
   ```
   [QuestPoint] Finalisation automatique de la quête
   [QuestManager] Terminer la quête
   [QuestManager] Distribution des récompenses
   ```

Si un log manque, c'est là que se situe le problème !

---

**Version** : v2.0 - Simplifié  
**Date** : 8 janvier 2026
