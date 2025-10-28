using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_ObjectiveDisplay : MonoBehaviour
{
    [Header("Objectifs Principaux (linéaires)")]
    public List<S_Objective> mainObjectives;

    [Header("Objectifs Secondaires (libres)")]
    public List<S_Objective> sideObjectives;

    [Header("UI")]
    public GameObject objectiveCanvas;
    public Text objectiveNameText;
    public Text objectiveDescriptionText;
    public Text objectiveStatusText;

    private int currentMainIndex = 0;

    void Start()
    {
        ChangeUIForNoObjective();
        SubscribeToObjectives();
        UpdateObjectiveUI();
    }

    /**
     * Abonne tous les objectifs à l’événement de complétion
     *
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, October 28th, 2025.
     * @return	void
     */
    void SubscribeToObjectives()
    {
        foreach (var obj in mainObjectives)
            obj.OnObjectiveCompleted.AddListener(OnObjectiveCompleted);

        foreach (var obj in sideObjectives)
            obj.OnObjectiveCompleted.AddListener(OnObjectiveCompleted);
    }

    /**
     * Appelé quand un objectif est terminé
     *
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, October 28th, 2025.
     * @return	void
     */
    void OnObjectiveCompleted()
    {
        // Si c’est un objectif principal et qu’il correspond à l’actuel
        if (currentMainIndex < mainObjectives.Count && mainObjectives[currentMainIndex].isCompleted)
        {
            currentMainIndex++;
            if (currentMainIndex < mainObjectives.Count)
                UpdateObjectiveUI();
            else
                HideObjectiveUI(); // plus d’objectifs
        }
        else
        {
            // Objectif secondaire terminé
            UpdateObjectiveUI();
        }
    }

    /**
     * Changer l’objectif secondaire suivi (choix du joueur)
     *
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, October 28th, 2025.
     * @access	public
     * @param	int	index	
     * @return	void
     */
    public void SelectSideObjective(int index)
    {
        if (index >= 0 && index < sideObjectives.Count)
        {
            DisplayObjective(sideObjectives[index]);
        }
    }

    /**
     * Met à jour l’UI pour l’objectif principal courant
     *
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, October 28th, 2025.
     * @return	void
     */
    void UpdateObjectiveUI()
    {
        ChangeUIForNoObjective();

        if (currentMainIndex < mainObjectives.Count)
        {
            DisplayObjective(mainObjectives[currentMainIndex]);
        }
        else
        {
            HideObjectiveUI();
        }
    }

    /**
     * Affiche un objectif précis
     *
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, October 28th, 2025.
     * @param	s_objective	objective	
     * @return	void
     */
    void DisplayObjective(S_Objective objective)
    {
        if (objectiveCanvas == null) return;
        objectiveCanvas.SetActive(true);

        objectiveNameText.text = objective.objectiveName;
        objectiveDescriptionText.text = objective.description;
        objectiveStatusText.text = objective.isCompleted ? "Completed" : "In Progress";
    }

    /**
     * Ajuste l’UI si aucun objectif principal n’est disponible
     *
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, October 28th, 2025.
     * @return	void
     */
    void ChangeUIForNoObjective()
    {
        if (objectiveCanvas != null)
            objectiveCanvas.SetActive(mainObjectives.Count > 0);
    }

    /**
     * Cache l’UI de l’objectif
     *
     * @author	Unknown
     * @since	v0.0.1
     * @return	void
     */
    void HideObjectiveUI()
    {
        if (objectiveCanvas != null)
            objectiveCanvas.SetActive(false);
    }

    /**
     * Méthode publique pour compléter l’objectif courant
     *
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, October 28th, 2025.
     * @access	public
     * @return	void
     */
    public void CompleteCurrentObjective()
    {
        if (currentMainIndex < mainObjectives.Count)
            mainObjectives[currentMainIndex].Complete();
    }
}
