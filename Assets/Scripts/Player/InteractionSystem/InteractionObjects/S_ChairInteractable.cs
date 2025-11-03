// ! Info sur le script : 
// ! appliquer le script sur l'objet où on veut s'asseoir


// TODO : faire une animation

using System.Numerics;
using UnityEngine;

public class S_ChairInteractable : MonoBehaviour, SI_Interactable
{
    private string interactText = "S'asseoir";
    [SerializeField] private GameObject player;

    private bool isPlayerSitting = false;


    // * Ne pas retirer ce qui est en desssous, nécessaire pour l'interface SI_Interactable
    // * ===================================================================================

    // ~ Méthode qui est activer quand on interagit avec l'objet
    public void Interact(Transform playerTransform)
    {   
        if (!isPlayerSitting)
        {
            isPlayerSitting = true;
            SitPlayer();
        }
        else
        {
            isPlayerSitting = false;
            UnsitPlayer();
        }
    }

    public string getInteractText()
    {
        return interactText;
    }

    public Transform getTransform()
    {
        return gameObject.transform;
    }

    // * ===================================================================================

    // Teleporte le joueur à la position assise
    void SitPlayer()
    {

        // milieu de la chaise
        UnityEngine.Vector3 chairPosition_Center = transform.position + new UnityEngine.Vector3(0, 0.5f, 0);


        player.transform.position = chairPosition_Center;

        // Bloquer les mouvements du joueur



        interactText = "Se lever";
        isPlayerSitting = true;
    }

    void UnsitPlayer()
    {

        // Mettre le joueur debout à coté de la chaise


        // Débloquer les mouvements du joueur

        interactText = "S'asseoir";
        isPlayerSitting = false;
    }

}
