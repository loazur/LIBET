using UnityEngine;

public class S_DoorInteractable : MonoBehaviour, SI_Interactable
{
    [SerializeField] private string interactText;
    

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact() //& Ouverture de la porte
    {
        Debug.Log("Ouverture porte");
    }

    public string getInteractText() //& Texte de la porte
    {
        return interactText;
    }

    public Transform getTransform() //& Position de la porte
    {
        return gameObject.transform;
    }


}
