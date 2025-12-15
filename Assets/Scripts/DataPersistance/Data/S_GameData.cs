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
    public Vector3 cameraRotation;
    public bool isCrouching;

    // Items
    public SerializedDictionary<string, Vector3> itemsPosition;
    public SerializedDictionary<string, Quaternion> itemsRotation;


    //& Constructeurs -> Contient les valeurs initiales
    public S_GameData()
    {
        // Joueur
        playerPosition = Vector3.zero;
        playerRotation = Quaternion.identity;
        isCrouching = false;
        cameraRotation = Vector3.zero;

        // Items
        itemsPosition = new SerializedDictionary<string, Vector3>();
        itemsRotation = new SerializedDictionary<string, Quaternion>();
    }
}
