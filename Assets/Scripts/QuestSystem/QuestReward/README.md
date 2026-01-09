# Système de Récompenses de Quête

## ⚠️ Configuration Importante - Auto-Complete Quest

**IMPORTANT** : Par défaut, les quêtes ne se terminent **pas automatiquement** après la dernière étape !

### Pour que les récompenses soient distribuées automatiquement :

1. Ouvrir le `SO_QuestInfo` de votre quête
2. Section **"Quest Completion"**  
3. ✅ Cocher **`Auto Complete Quest`**

✅ **Avec Auto Complete** : Les récompenses sont données dès que toutes les étapes sont terminées  
❌ **Sans Auto Complete** : Le joueur doit retourner au QuestPoint pour terminer la quête

---

## Utilisation de la Lucidity Reward

1. Clic droit dans Unity → `Create > Quest System > Rewards > Lucidity Reward`
2. Configurer la valeur `lucidityAmount` (ex: 10 pour +10%)
3. Ajouter la récompense dans le tableau `questRewards` de votre `SO_QuestInfo`
4. ✅ Activer `Auto Complete Quest` dans le `SO_QuestInfo`