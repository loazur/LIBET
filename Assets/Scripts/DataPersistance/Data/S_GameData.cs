using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class S_GameData 
{
    //~ Données à sauvegarder
    // Joueur
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    // Items
    public SerializedDictionary<string, Vector3> itemsPosition;
    public Dictionary<string, Quaternion> itemsRotation;


    //& Constructeurs -> Contient les valeurs initiales
    public S_GameData()
    {
        // Joueur
        playerPosition = Vector3.zero;
        playerRotation = Quaternion.identity;

        // Items
        itemsPosition = new SerializedDictionary<string, Vector3>();
        itemsRotation = new SerializedDictionary<string, Quaternion>();
    }
}
