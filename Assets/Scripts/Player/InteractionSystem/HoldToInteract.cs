using UnityEngine;

public class HoldToInteract : MonoBehaviour
{
    //! HoldToInteract permet d'etre attaché à un gameObject et d'ainsi obligé le joueur à maintenir si il veut l'utiliser

    public float howLongToHold; // Combien de temps faut tenir le bouton pour lancer
    [HideInInspector] public float holdTimer;
}
