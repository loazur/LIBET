using UnityEngine;

public class S_SenseOfMotionEvent : MonoBehaviour
{
    //~ SenseOfMotionEvent -> Change le FOV et la vitesse du joueur pour donner l'illusion qu'il n'avance pas
     
    //TODO vrai event

    void OnEnable() //& Lors de l'activation de l'event
    {   
        S_CameraSettingsData.instance.setCurrentFieldOfView(120);
        Debug.Log("Event activé!");
    }

    void OnDisable() //& Lors de la desactivation de l'event
    {
        S_CameraSettingsData.instance.resetCurrentFieldOfView();
        Debug.Log("Event désactivé");
    }


}
