// ! Info sur le script : 
// ! appliquer le script sur l'objet où on veut s'asseoir

using UnityEngine;

public class S_ChairInteractable : MonoBehaviour, SI_Interactable
{
    [SerializeField] private string interactText;
    [SerializeField] private GameObject player;

    private bool isPlayerSitting = false;


    // * Ne pas retirer ce qui est en desssous, nécessaire pour l'interface SI_Interactable
    // * ===================================================================================

    // ~ Méthodes qui est activer quand on interagit avec l'objet
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
        player.transform.position = transform.position;
    }

    void UnsitPlayer()
    {
        // Logique pour faire se lever le joueur de la chaise
        Debug.Log("Le joueur se lève de la chaise");
    }

}
