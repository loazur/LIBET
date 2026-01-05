# Système AlzheimerEvents

Ce système gère les événements liés à l'Alzheimer dans le jeu LIBET. Il simule la perte progressive de lucidité du personnage et déclenche des événements perturbateurs basés sur cette jauge.

---

## 📋 Table des matières

1. [Vue d'ensemble](#vue-densemble)
2. [Jauge de Lucidité](#jauge-de-lucidité)
3. [Paliers d'intensité](#paliers-dintensité)
4. [Système d'Events](#système-devents)
5. [Cycle Alzheimer](#cycle-alzheimer)
6. [Créer un nouvel Event](#créer-un-nouvel-event)
7. [API Publique](#api-publique)
8. [Debug](#debug)

---

## Vue d'ensemble

Le système fonctionne autour de 3 concepts principaux :

```
┌─────────────────────────────────────────────────────────────┐
│                    JAUGE DE LUCIDITÉ                        │
│  100% ════════════════════════════════════════════════ 0%   │
│   │                    │                    │           │   │
│   │    LUCIDE (60-100) │  CONFUS (20-60)   │ TRÈS (0-20)│   │
│   │    Aucun event     │  Events actifs    │ Max events │   │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                   INTERVALLE DYNAMIQUE                       │
│  Lucidité haute → Intervalle long (180s)                    │
│  Lucidité basse → Intervalle court (30s)                    │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                    SÉLECTION PONDÉRÉE                        │
│  Chaque event a un poids de base                            │
│  Poids ajusté = baseWeight × lucidityMult × cycleMult       │
└─────────────────────────────────────────────────────────────┘
```

---

## Jauge de Lucidité

La jauge de lucidité représente l'état mental du personnage (0-100%).

### Paramètres configurables

| Paramètre | Description | Valeur par défaut |
|-----------|-------------|-------------------|
| `lucidity` | Valeur actuelle de la jauge | 100% |
| `eventActivationThreshold` | Seuil sous lequel les events se déclenchent | 60% |
| `lucidityDecreaseRate` | Diminution par seconde | 0.5 |
| `autoDecreaseLucidity` | Diminution automatique activée | true |

### Comportement

- **Au-dessus de 60%** : Aucun event ne se déclenche
- **En dessous de 60%** : Les events commencent à se déclencher
- **Plus c'est bas** : Plus les events sont fréquents et intenses
- **Remontée au-dessus de 60%** : Tous les events actifs sont désactivés

### Méthodes

```csharp
// Définir la lucidité
S_AlzheimerEventsManager.Instance.SetLucidity(50f);

// Modifier relativement
S_AlzheimerEventsManager.Instance.ModifyLucidity(-10f); // -10%

// Récupérer de la lucidité
S_AlzheimerEventsManager.Instance.RecoverLucidity(20f); // +20%

// Diminuer la lucidité
S_AlzheimerEventsManager.Instance.DecreaseLucidity(15f); // -15%
```

---

## Paliers d'intensité

Les paliers définissent le comportement du système selon le niveau de lucidité.

### Paliers par défaut

| Palier | Lucidité | Multiplicateur | Max Events |
|--------|----------|----------------|------------|
| Lucide | 60-100% | 0x | 0 |
| Légèrement confus | 40-60% | 1x | 1 |
| Confus | 20-40% | 1.5x | 2 |
| Très confus | 0-20% | 2x | 3 |

### Effet du multiplicateur

- **Sur l'intensité** : `intensité finale = baseIntensity × multiplier × lucidityFactor`
- **Sur le poids** : Augmente les chances de déclenchement

---

## Système d'Events

### Intervalle dynamique

L'intervalle entre les tentatives d'events est calculé dynamiquement :

```
intervalle = lerp(minInterval, maxInterval, lucidity/threshold) × cycleModifier
```

| Lucidité | Intervalle approximatif |
|----------|------------------------|
| 60% | ~180 secondes |
| 40% | ~120 secondes |
| 20% | ~60 secondes |
| 0% | ~30 secondes |

Le **cycle Alzheimer** réduit encore l'intervalle de 10% par cycle (minimum 30%).

### Types d'activation

| Type | Description |
|------|-------------|
| `Random` | Se déclenche aléatoirement selon le poids |
| `OnWakeUp` | Se déclenche quand Libet se réveille |
| `OnThreshold` | Se déclenche à un palier spécifique |
| `Story` | Event d'histoire (one-shot automatique) |
| `Manual` | Déclenché uniquement par script |

### Sélection pondérée

Chaque event a un **poids de base** (`baseWeight`). Le poids ajusté est calculé :

```csharp
// Plus la lucidité est basse, plus le poids augmente
lucidityMultiplier = 1 + max(0, (60 - lucidity) / 30)

// Chaque cycle augmente les chances
cycleMultiplier = 1 + (cycle × 0.2)

adjustedWeight = baseWeight × lucidityMultiplier × cycleMultiplier
```

**Exemple** : Un event avec `baseWeight = 1.0` à 30% de lucidité, cycle 2 :
- `lucidityMultiplier = 1 + (60-30)/30 = 2.0`
- `cycleMultiplier = 1 + (2 × 0.2) = 1.4`
- `adjustedWeight = 1.0 × 2.0 × 1.4 = 2.8`

---

## Cycle Alzheimer

Le cycle représente la progression de la maladie au fil du temps.

### Fonctionnement

- Le cycle augmente quand la lucidité passe sous `cycleThreshold` (20% par défaut)
- Chaque cycle :
  - Réduit l'intervalle entre events de 10%
  - Augmente le poids des events de 20%

### Impact

| Cycle | Intervalle | Poids events |
|-------|------------|--------------|
| 0 | 100% | 100% |
| 1 | 90% | 120% |
| 2 | 80% | 140% |
| 3 | 70% | 160% |
| ... | min 30% | ... |

---

## Créer un nouvel Event

### 1. Créer le ScriptableObject

1. Clic droit dans le dossier `Events/` → **Create > Alzheimer > Event**
2. Configurer les paramètres :

```
eventName: "MonEvent"
eventDescription: "Description de l'effet"
activationType: Random
baseWeight: 1.0
isOneShot: false
minLucidityThreshold: 0
maxLucidityThreshold: 60
baseIntensity: 1.0
duration: 30 (0 = permanent)
canStack: false
priority: 5
eventPrefab: [Glisser le prefab]
```

### 2. Créer le script de l'event

```csharp
using UnityEngine;

public class S_MonEvent : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private SO_AlzheimerEvent eventData;

    void OnEnable()
    {
        // Applique l'effet
        // Utilise eventData.currentIntensity pour l'intensité dynamique
        float intensity = eventData != null ? eventData.currentIntensity : 1f;
        ApplyEffect(intensity);
    }

    void OnDisable()
    {
        // Annule l'effet
        ResetEffect();
    }

    void OnDestroy()
    {
        ResetEffect();
    }

    private void ApplyEffect(float intensity)
    {
        // Ton code ici
    }

    private void ResetEffect()
    {
        // Restaure l'état original
    }
}
```

### 3. Créer le Prefab

1. Créer un GameObject vide
2. Ajouter le script `S_MonEvent`
3. Sauvegarder en Prefab dans le dossier de l'event
4. Référencer le Prefab dans le ScriptableObject

### 4. Ajouter à la liste

Ajouter le ScriptableObject à la liste `availableEvents` du `S_AlzheimerEventsManager`.

---

## API Publique

### Propriétés

```csharp
// Accès au singleton
S_AlzheimerEventsManager.Instance

// Propriétés en lecture
float lucidity = Instance.Lucidity;           // 0-100
int cycle = Instance.CurrentCycle;            // 0+
LucidityTier tier = Instance.CurrentTier;     // Palier actuel
int count = Instance.ActiveEventsCount;       // Nombre d'events actifs
bool active = Instance.EventsAreActive;       // Si lucidité < seuil
```

### Méthodes principales

```csharp
// Lucidité
Instance.SetLucidity(float value);
Instance.ModifyLucidity(float delta);
Instance.RecoverLucidity(float amount);
Instance.DecreaseLucidity(float amount);

// Events
Instance.ActivateEvent(SO_AlzheimerEvent evt);
Instance.ForceActivateEvent(SO_AlzheimerEvent evt);
Instance.DeactivateEvent(SO_AlzheimerEvent evt);
Instance.DeactivateAllEvents();
Instance.TryTriggerRandomEvent();

// Events spéciaux
Instance.TriggerWakeUpEvents();
Instance.TriggerStoryEvent("NomEvent");

// Contrôle des boucles
Instance.StartEventLoop();
Instance.StopEventLoop();
Instance.StartLucidityDecrease();
Instance.StopLucidityDecrease();

// Utilitaires
Instance.ResetAllEventsState();
List<SO_AlzheimerEvent> actifs = Instance.GetActiveEventsList();
float interval = Instance.GetDynamicEventInterval();
```

---

## Debug

### ContextMenu (Clic droit sur le composant)

| Menu | Action |
|------|--------|
| `Debug/Afficher État Complet` | Log complet de l'état du système |
| `Debug/Lucidité -10%` | Diminue la lucidité de 10% |
| `Debug/Lucidité -25%` | Diminue la lucidité de 25% |
| `Debug/Lucidité +10%` | Augmente la lucidité de 10% |
| `Debug/Lucidité +25%` | Augmente la lucidité de 25% |
| `Debug/Lucidité = 0%` | Met la lucidité à 0% |
| `Debug/Lucidité = 50%` | Met la lucidité à 50% |
| `Debug/Lucidité = 100%` | Met la lucidité à 100% |
| `Debug/Forcer Event Aléatoire` | Force le déclenchement d'un event |
| `Debug/Désactiver Tous les Events` | Désactive tous les events actifs |
| `Debug/Lister Events Éligibles` | Affiche les events pouvant se déclencher |
| `Debug/Cycle +1` | Augmente le cycle de 1 |
| `Debug/Reset Cycle` | Remet le cycle à 0 |
| `Debug/Toggle Diminution Auto` | Active/désactive la diminution auto |
| `Debug/Reset États Events` | Réinitialise les états hasTriggered |

### Logs colorés

Les logs utilisent des couleurs pour faciliter le debug :

- 🟡 `<color=yellow>` : Changements de palier, informations
- 🟢 `<color=green>` : Récupération de lucidité
- 🔴 `<color=red>` : Perte de lucidité, cycles
- 🔵 `<color=cyan>` : Activation d'events
- 🟣 `<color=magenta>` : Events forcés
- ⚪ `<color=gray>` : Désactivation d'events
- 🟠 `<color=orange>` : Avertissements

---

## Structure des fichiers

```
AlzheimerEvents/
├── README.md                      # Cette documentation
├── S_AlzheimerEventsManager.cs    # Manager principal
├── SO_AlzheimerEvent.cs           # ScriptableObject des events
└── Events/
    ├── DepthPerceptionShadowLoss/
    │   ├── DepthPerceptionShadowLoss.asset
    │   └── S_DepthPerceptionShadowLoss.cs
    └── SenseOfMotion/
        ├── SenseOfMotionEvent.asset
        └── S_SenseOfMotionEvent.cs
```

---

## Exemple d'utilisation

```csharp
// Dans un autre script, par exemple quand le joueur prend un médicament
public class S_Medication : MonoBehaviour
{
    public void TakeMedication()
    {
        // Récupère 30% de lucidité
        S_AlzheimerEventsManager.Instance.RecoverLucidity(30f);
    }
}

// Quand le joueur subit un stress
public class S_StressfulEvent : MonoBehaviour
{
    public void OnStress()
    {
        // Perd 15% de lucidité
        S_AlzheimerEventsManager.Instance.DecreaseLucidity(15f);
    }
}

// Quand Libet se réveille
public class S_WakeUpManager : MonoBehaviour
{
    public void OnWakeUp()
    {
        S_AlzheimerEventsManager.Instance.TriggerWakeUpEvents();
    }
}
```
