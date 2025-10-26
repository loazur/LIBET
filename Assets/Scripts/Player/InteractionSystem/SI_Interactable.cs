using UnityEngine;

public interface SI_Interactable
{
    //~ Interface à mettre sur chaque objet avec lequelle on peut interagir
    void Interact(Transform playerTransform); //& Interaction avec l'objet
    string getInteractText(); //& Texte affiché sur l'UI
    Transform getTransform(); //& Position de l'objet
}
