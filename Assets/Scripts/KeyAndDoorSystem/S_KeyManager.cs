using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Gestionnaire singleton pour tracker toutes les clés collectées par le joueur.
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Monday, January 5th, 2026.
 * @global
 */
public class S_KeyManager : MonoBehaviour, SI_DataPersistance
{
    //~ Singleton
    public static S_KeyManager instance { get; private set; }

    //~ Dictionnaire: doorID -> liste des clés collectées pour cette porte
    private Dictionary<string, List<string>> collectedKeys = new Dictionary<string, List<string>>();

    //~ Event déclenché quand une clé est collectée (doorID, keyID)
    public event Action<string, string> OnKeyCollected;

    void Awake()
    {
        //& Setup Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //!---------------- SI_DataPersistance ----------------

    //~ Sauvegarde clés obtenus

    public void LoadData(S_GameData gameData)
    {
        // Récupérer les clés stockés
        collectedKeys.Clear();

        foreach (KeyValuePair<string, List<string>> eachNote in gameData.collectedKeys)
        {
            collectedKeys.Add(eachNote.Key, eachNote.Value);
        }
    }

    public void SaveData(S_GameData gameData)
    {
        // Sauvegarder les clés actuels
        gameData.collectedKeys.Clear();

        foreach (KeyValuePair<string, List<string>> eachNote in collectedKeys)
        {
            gameData.collectedKeys.Add(eachNote.Key, eachNote.Value);
        }
    }

    public int GetLoadPriority() => 0; // ✅ Priorité normale


    
    /**
     * Enregistre une clé comme collectée pour une porte donnée.clé
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 5th, 2026.
     * @access	public
     * @param	string	doorID	
     * @param	string	keyID 	
     * @return	void
     */
    public void CollectKey(string doorID, string keyID)
    {
        if (!collectedKeys.ContainsKey(doorID))
        {
            collectedKeys[doorID] = new List<string>();
        }

        if (!collectedKeys[doorID].Contains(keyID))
        {
            collectedKeys[doorID].Add(keyID);
            Debug.Log($"[KeyManager] Clé '{keyID}' collectée pour la porte '{doorID}'");
            
            OnKeyCollected?.Invoke(doorID, keyID);
        }
    }

    /**
     * Vérifie si le joueur a collecté toutes les clés requises pour une porte.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 5th, 2026.
     * @access	public
     * @param	string	doorID          	
     * @param	int   	requiredKeyCount	
     * @return	mixed
     */
    public bool HasAllKeys(string doorID, int requiredKeyCount)
    {
        if (!collectedKeys.ContainsKey(doorID))
        {
            return false;
        }

        return collectedKeys[doorID].Count >= requiredKeyCount;
    }

    /**
     * Retourne le nombre de clés collectées pour une porte donnée.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 5th, 2026.
     * @access	public
     * @param	string	doorID	
     * @return	mixed
     */
    public int GetCollectedKeyCount(string doorID)
    {
        if (!collectedKeys.ContainsKey(doorID))
        {
            return 0;
        }

        return collectedKeys[doorID].Count;
    }

    
    /**
     * Vérifie si une clé spécifique a été collectée.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 5th, 2026.
     * @access	public
     * @param	string	doorID	
     * @param	string	keyID 	
     * @return	mixed
     */
    public bool HasKey(string doorID, string keyID)
    {
        if (!collectedKeys.ContainsKey(doorID))
        {
            return false;
        }

        return collectedKeys[doorID].Contains(keyID);
    }

    /**
     * Réinitialise toutes les clés collectées (utile pour restart de niveau).
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 5th, 2026.
     * @access	public
     * @return	void
     */
    public void ResetAllKeys()
    {
        collectedKeys.Clear();
        Debug.Log("[KeyManager] Toutes les clés ont été réinitialisées");
    }

    /**
     * Réinitialise les clés pour une porte spécifique.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 5th, 2026.
     * @access	public
     * @param	string	doorID	
     * @return	void
     */
    public void ResetKeysForDoor(string doorID)
    {
        if (collectedKeys.ContainsKey(doorID))
        {
            collectedKeys[doorID].Clear();
            Debug.Log($"[KeyManager] Clés réinitialisées pour la porte '{doorID}'");
        }
    }
}
