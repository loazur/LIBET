using UnityEngine;

// & Classe pour gérer les objectifs dans le jeu
public class S_Objective : MonoBehaviour
{
    //! Variables 

    public string objectiveName; // Nom de l'objectif
    public string description; // Description de l'objectif
    public bool isCompleted; // Statut de l'objectif

    // Liste des objectifs suivants
    

    // Canvas pour afficher l'objectif
    public GameObject objectiveCanvas;

    //! Méthodes
    void Start()
    {
        // Initialisation de l'objectif
        isCompleted = false;
        UpdateObjectiveUI();
    }


    /**
     * Mettre à jour l'interface utilisateur de l'objectif
     *
     * @return	void
     */
    void UpdateObjectiveUI()
    {
        if (objectiveCanvas != null)
        {
            // Mettre à jour le texte de l'objectif
            UnityEngine.UI.Text[] texts = objectiveCanvas.GetComponentsInChildren<UnityEngine.UI.Text>();
            foreach (var text in texts)
            {
                if (text.gameObject.name == "ObjectiveName")
                {
                    text.text = objectiveName;
                }
                else if (text.gameObject.name == "ObjectiveDescription")
                {
                    text.text = description;
                }
                else if (text.gameObject.name == "ObjectiveStatus")
                {
                    text.text = isCompleted ? "Completed" : "In Progress";
                }
            }
        }
    }

    /**
     * Méthode pour marquer l'objectif comme complété
     *
     * @access	public
     * @return	void
     */
    
}
