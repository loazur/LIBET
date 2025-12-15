using System.IO;
using UnityEngine;

public class S_FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false; // Cryptée les données ou non
    private readonly string encryptionCodeWord = "libetKey!"; // Clé

    //& Constructeur
    public S_FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    //!-----------------------------------------

    public S_GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
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
            catch(System.Exception e)
            {
                Debug.LogError("Erreur lors du chargement des données dans le fichier: " + fullPath + "\n" + e);
                throw;
            }
        }

        return loadedGameData;
    }

    public void Save(S_GameData gameData)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

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
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de la sauvegarde des données dans le fichier: " + fullPath + "\n" + e);
            throw;
        }
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
}
