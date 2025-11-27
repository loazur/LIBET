using UnityEngine;

public class S_SenseOfMotionEvent : MonoBehaviour
{
    //~ SenseOfMotionEvent -> Change le FOV et la vitesse du joueur pour donner l'illusion qu'il n'avance pas

    //TODO Changer vraiment tout ce qu'il faut et de manière agréable avec un Lerp + BLOQUER lE CHANGEMENT DU FOV DANS LES SETTINGS CAMERA
    
    void OnEnable() //& Activation de l'event
    {   
        S_CameraSettingsData.instance.setCurrentFieldOfView(120);
    }

    void OnDisable() //& Désactivation de l'event
    {
        S_CameraSettingsData.instance.resetCurrentFieldOfView();
    }


}
