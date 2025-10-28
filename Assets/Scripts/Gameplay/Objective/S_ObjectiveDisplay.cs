using System.Collections.Generic;
using UnityEngine;

// & Classe pour gérer les objectifs dans le jeu
public class S_ObjectiveDisplay : MonoBehaviour
{
    //! Variables 



    // Liste des objectifs 
    public List<S_Objective> principale_objectives; //& Le faire en File peut-être ?
    public List<S_Objective> side_objectives; //& Ne pas fair en File car le joueur peut choisir l'ordre
    private int currentObjectiveIndex = 0;
    // Canvas pour afficher l'objectif
    public GameObject objectiveCanvas;

    //! Méthodes
    void Start()
    {
        // Initialisation de l'objectif
        principale_objectives[currentObjectiveIndex].isCompleted = false;
        UpdateObjectiveUI();
    }

    void Update()
    {
        // Vérifier si les objectifs sont complétés
        for (int i = 0; i < principale_objectives.Count; i++)
        {
            if (principale_objectives[i].isCompleted && i == currentObjectiveIndex)
            {
                currentObjectiveIndex++;
                if (currentObjectiveIndex < principale_objectives.Count)
                {
                    UpdateObjectiveUI();
                }
                // & Sinon rien
            }
        }
    }

    /**
     * vérifie s'il y a un objectif
     *
     * @access	private
     * @return	void
     */
    private void ChangeUIForNoObjective()
    {
        if (principale_objectives.Count == 0)
        {
            if (objectiveCanvas != null)
            {
                objectiveCanvas.SetActive(false);
            }
        }
        else
        {
            if (objectiveCanvas != null)
            {
                objectiveCanvas.SetActive(true);
            }
        }
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
                    text.text = principale_objectives[currentObjectiveIndex].objectiveName;
                }
                else if (text.gameObject.name == "ObjectiveDescription")
                {
                    text.text = principale_objectives[currentObjectiveIndex].description;
                }
                else if (text.gameObject.name == "ObjectiveStatus")
                {
                    text.text = principale_objectives[currentObjectiveIndex].isCompleted ? "Completed" : "In Progress";
                }
            }
        }
    }

    /**
     * Objectif compléter
     *
     * @access	public
     * @return	void
     */
    public void CompleteObjective()
    {
        principale_objectives[currentObjectiveIndex].isCompleted = true;
        UpdateObjectiveUI();
    }
    
}
