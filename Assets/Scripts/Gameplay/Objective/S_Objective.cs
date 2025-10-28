using UnityEngine;

// & Classe pour gérer les objectifs dans le jeu
public class S_Objective : MonoBehaviour
{
    //! Variables 

    public string objectiveName; // Nom de l'objectif
    public string description; // Description de l'objectif
    public bool isCompleted; // Statut de l'objectif

    // porchain objectif (optionnel)
    public S_Objective nextObjective;

    // Canvas pour afficher l'objectif
    public GameObject objectiveCanvas;

    //! Méthodes
    void Start()
    {
        // Initialisation de l'objectif
        isCompleted = false;
        UpdateObjectiveUI();
    }

    void UpdateObjectiveUI()
    {
        // Mettre à jour l'interface utilisateur de l'objectif
        if (objectiveCanvas != null)
        {
            // Exemple : Mettre à jour le texte de l'objectif
            // objectiveCanvas.GetComponentInChildren<Text>().text = description + (isCompleted ? " (Complété)" : "");
        }
    }

    /**
     * Méthode pour marquer l'objectif comme complété
     *
     * @access	public
     * @return	void
     */
    public void CompleteObjective()
    {
        isCompleted = true;
        UpdateObjectiveUI();

        // Activer le prochain objectif s'il existe
        if (nextObjective != null)
        {
            nextObjective.gameObject.SetActive(true);
        }
    }
}
