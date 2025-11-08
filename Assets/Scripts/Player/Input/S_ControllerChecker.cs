using System.Collections;
using UnityEngine;

public class S_ControllerChecker : MonoBehaviour
{
    //~ Variable qui sert à savoir si un controller est connecté à l'ordinateur du joueur
    private bool isConnected = false;

    //TODO A améliorer pour que ça check ce que le joueur utilise plus efficacement (au lieu de regarder si une manette est branché)

    void Awake()
    {
        StartCoroutine(CheckForControllers());
    }

    //! --------------- Fonctions privés ---------------

    IEnumerator CheckForControllers()
    {
        while (true)
        {
            var controllers = Input.GetJoystickNames();

            if (!isConnected && controllers.Length > 0)
            {
                isConnected = true;

            }
            else if (isConnected && controllers.Length == 0)
            {
                isConnected = false;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    //? ------------------------------------------------   

    public bool isUsingController()
    {
        return isConnected;
    }
}



