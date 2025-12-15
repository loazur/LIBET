using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_AlzheimerEventsManager : MonoBehaviour
{
    [Header("Gestion des events du jeu")]
    [Tooltip("Liste des events activable")]
    [SerializeField] public List<SO_AlzheimerEvent> alzheimerEvents = new List<SO_AlzheimerEvent>();

    [Tooltip("Tout les combiens de temps un event s'active en secondes")]
    [SerializeField] private float activationInterval = 180f;

    [Tooltip("Si la boucle d'event aléatoire est active")]
    [SerializeField] private bool eventLoopActive = true;

    void Start()
    {   
        if (eventLoopActive) // Lancement de la boucle
            StartCoroutine(EventLoop());
    }

    private IEnumerator EventLoop() //& Boucle des events aléatoire
    {
        while (true)
        {
            yield return new WaitForSeconds(activationInterval);
            TriggerRandomEvent();
        }
    }

    private void TriggerRandomEvent() //& Lance un event aléatoire en fonction du poids
    {
        if (alzheimerEvents.Count == 0) return;

        // Calcul du total des poids
        float totalWeight = 0;
        foreach (var alzheimerEvent in alzheimerEvents)
        {
            if (!(alzheimerEvent.eventIsOneShot && alzheimerEvent.eventHasTriggered && alzheimerEvent.eventActivationType == SO_AlzheimerEvent.ActivationType.Randomly))
                totalWeight += alzheimerEvent.eventBaseWeight; // Ajoute au poids total, si non oneshot/déja lancé et si bien Randomly
        }

        if (totalWeight == 0) return; // Aucun events

        // Gére l'event en fonction du poits
        float randomValue = Random.Range(0, totalWeight);
        float currentSum = 0;

        foreach (var alzheimerEvent in alzheimerEvents)
        {
            if (alzheimerEvent.eventIsOneShot && alzheimerEvent.eventHasTriggered) // Gestion OneShot
                continue;

            if (alzheimerEvent.eventActivationType != SO_AlzheimerEvent.ActivationType.Randomly) // Gestion ActivationType
                continue;

            currentSum += alzheimerEvent.eventBaseWeight;

            if (randomValue < currentSum) // Lance l'event aléatoire
            {
                alzheimerEvent.Trigger(); 
                return;
            }
        }
        

    }

    private void TriggerSpecificEvent(SO_AlzheimerEvent alzheimerEvent) //& Lance un event spécifique
    {
        if (alzheimerEvent.eventIsOneShot && alzheimerEvent.eventHasTriggered) return; // Si s'active qu'une fois et c'est deja lancé

        alzheimerEvent.Trigger(); // Lance l'event
    }

}
