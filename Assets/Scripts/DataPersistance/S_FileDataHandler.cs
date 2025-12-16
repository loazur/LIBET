using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class S_FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false; // Cryptée les données ou non
    private readonly string encryptionCodeWord = "libetKey!"; // Clé
    private readonly string backupExtension = ".bak";

    //& Constructeur
    public S_FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    //!-----------------------------------------

    public S_GameData Load(string profileId, bool allowRestoreFromBackup = true)
    {
        // Si profileId = null
        if (profileId == null)
        {
            return null;
        }

        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        S_GameData loadedGameData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                // Chargement des données
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Decryptage des données
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // Deserialization des données du JSON vers un objet C#
                loadedGameData = JsonUtility.FromJson<S_GameData>(dataToLoad);
            }
            catch(Exception e)
            {
                if (allowRestoreFromBackup)
                {
                    Debug.LogWarning("Erreur lors du chargement des données dans le fichier: " + fullPath + "\n" + e + " Debut d'une backup.");

                    bool rollbackSuccess = AttemptRollback(fullPath);
                    if (rollbackSuccess)
                    {
                        loadedGameData = Load(profileId, false);
                    }
                }
                else
                {
                    Debug.LogError("Erreur lors du chargement des données dans le fichier: " + fullPath + "\n" + e + " La backup n'a pas marché.");

                }
            }
        }

        return loadedGameData;
    }

    public void Save(S_GameData gameData, string profileId)
    {
        // Si profileId = null
        if (profileId == null)
        {
            return;
        }
        
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        string backupFilePath = fullPath + backupExtension;

        try
        {
            // Création du directory path si il n'existe pas
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialization des données C# en JSON
            string dataToStore = JsonUtility.ToJson(gameData, true);

            // Encryptage optionnel des données
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            // Ecriture des données dans le fichier
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }

                // Création d'une backup au cas ou&
                S_GameData verifiedGameData = Load(profileId);

                if (verifiedGameData != null)
                {
                    File.Copy(fullPath, backupFilePath, true);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de la sauvegarde des données dans le fichier: " + fullPath + "\n" + e);
            throw;
        }
    }

    public Dictionary<string, S_GameData> LoadAllProfiles()
    {
        Dictionary<string, S_GameData> profileDictionary = new Dictionary<string, S_GameData>();

        // Boucle dans tout les dossiers des sauvegardes
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;

            // Vérification si le fichier de data existe, sinon c'est pas un dossier de sauvegarde
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            if (!File.Exists(fullPath))
            {
                // Pas un dossier de sauvegarde
                continue;
            }

            // Charge les données de ce profile et les met dans le dictionaire
            S_GameData profileData = Load(profileId);

            // Au cas ou on regarde si c'est null
            if (profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
        }

        return profileDictionary;
    }

    public void Delete(string profileId)
    {
        // Si profileId = null
        if (profileId == null)
        {
            return; 
        }

        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        
        if (File.Exists(fullPath))
        {
            try
            {
                Directory.Delete(Path.GetDirectoryName(fullPath), true); // Supprime le dossier recursivement
                Debug.Log("Dossier de sauvegarde supprimé : " + fullPath);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Erreur lors de la suppression du dossier : " + fullPath + "\n" + e);
            }
        }
        else
        {
            Debug.LogWarning("Aucun dossier à supprimer : " + fullPath);
        }
    }

    public string GetMostRecentlyUpdatedProfileId()
    {
        string mostRecentProfileId = null;

        Dictionary<string, S_GameData> profilesGameData = LoadAllProfiles();
        foreach(KeyValuePair<string, S_GameData> pair in profilesGameData)
        {
            string profileId = pair.Key;
            S_GameData gameData = pair.Value;

            // Skip si null
            if (gameData == null)
            {
                continue;
            }

            // Si c'est la 1er data alors c'est la plus récente
            if (mostRecentProfileId == null)
            {
                mostRecentProfileId = profileId;
            }
            else // Sinon on la compare en fonction de la date
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);

                // Le plus grand des deux sera le plus récent
                if (newDateTime > mostRecentDateTime)
                {
                    mostRecentProfileId = profileId;
                }
            }
        }

        return mostRecentProfileId;
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; ++i)
        {
            modifiedData += (char) (data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }

        return modifiedData;
    }

    private bool AttemptRollback(string fullPath)
    {
        bool success = false;

        string backupFilePath = fullPath + backupExtension;

        try
        {
            // Backup
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullPath, true);
                success = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Erreur lors du backup: " + e);
        }

        return success;
    }

}
