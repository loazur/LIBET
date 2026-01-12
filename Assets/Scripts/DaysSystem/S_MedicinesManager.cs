using System;
using System.Collections.Generic;
using UnityEngine;

public class S_MedicinesManager : MonoBehaviour
{
    //! S_MedicinesManager gère le spawn des médicaments et la gestion des stocks

    public static S_MedicinesManager instance { get; private set; }

    [Header("Medicine Spawning")]
    [SerializeField] private GameObject medicinePrefab; // Prefab du médicament
    [SerializeField] private Transform[] spawnPoints; // Points de spawn possibles pour les médicaments

    [Header("Storage Settings")]
    [SerializeField] private int maxStoredMedicines = 1; // Maximum de médicaments stockables pour le jour suivant

    //~ Gestion des médicaments
    private List<GameObject> spawnedMedicines = new List<GameObject>(); // Médicaments spawned dans la scène
    private int medicinesNotEaten = 0; // Médicaments non mangés du jour actuel
    private int storedMedicinesFromPreviousDay = 0; // Médicaments gardés du jour précédent

    //~ Events
    public event Action<int> OnMedicineEaten; // int = nombre de médicaments restants dans la scène

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //! ---------- Génération des médicaments ----------

    public void GenerateMedicines(int newMedicinesCount, int medicinesPerDayLimit)
    {
        // Nettoyer les anciens médicaments restants
        CleanupRemainingMedicines();

        // Ajuster le nombre de nouveaux médicaments en fonction des stockés
        int adjustedNewMedicines = CalculateAdjustedMedicines(newMedicinesCount, medicinesPerDayLimit);

        // Calculer le total de médicaments à spawner
        int totalMedicines = adjustedNewMedicines + storedMedicinesFromPreviousDay;

        Debug.Log($"Génération de {totalMedicines} médicaments : {adjustedNewMedicines} nouveaux + {storedMedicinesFromPreviousDay} stockés du jour précédent (limite: {medicinesPerDayLimit})");

        // Réinitialiser le compteur
        medicinesNotEaten = totalMedicines;
        storedMedicinesFromPreviousDay = 0; // Reset car déjà utilisés

        // Spawner les médicaments dans la scène
        SpawnMedicinesInWorld(totalMedicines);
    }

    private int CalculateAdjustedMedicines(int requestedNewMedicines, int medicinesPerDayLimit) //& Re-calcule en tenant compte des stocks
    {
        // Calculer combien de places sont disponibles après les médicaments stockés
        int availableSlots = medicinesPerDayLimit - storedMedicinesFromPreviousDay;

        // Limiter les nouveaux médicaments aux places disponibles
        int adjustedMedicines = Mathf.Min(requestedNewMedicines, availableSlots);

        // S'assurer qu'on ne génère jamais de nombre négatif
        adjustedMedicines = Mathf.Max(0, adjustedMedicines);

        if (adjustedMedicines < requestedNewMedicines)
        {
            Debug.LogWarning($"Réduction des nouveaux médicaments de {requestedNewMedicines} à {adjustedMedicines} (stockés: {storedMedicinesFromPreviousDay}, limite: {medicinesPerDayLimit})");
        }

        return adjustedMedicines;
    }

    private void SpawnMedicinesInWorld(int count)
    {
        if (count <= 0)
        {
            Debug.Log("Aucun médicament à spawner (limite atteinte avec les médicaments stockés)");
            return;
        }

        if (medicinePrefab == null)
        {
            Debug.LogError("Medicine prefab non assigné !");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Aucun spawn point assigné pour les médicaments !");
            return;
        }

        // Sélectionner des spawn points aléatoires
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
            availableIndices.Add(i);

        int spawnedCount = 0;
        for (int i = 0; i < count && availableIndices.Count > 0; i++)
        {
            // Choisir un spawn point aléatoire
            int randomIndex = UnityEngine.Random.Range(0, availableIndices.Count);
            int spawnIndex = availableIndices[randomIndex];
            availableIndices.RemoveAt(randomIndex);

            // Spawner le médicament
            Transform spawnPoint = spawnPoints[spawnIndex];
            GameObject medicine = Instantiate(medicinePrefab, spawnPoint.position, spawnPoint.rotation);
            spawnedMedicines.Add(medicine);
            spawnedCount++;

            Debug.Log($"Médicament spawné à {spawnPoint.name}");
        }

        if (spawnedCount < count)
        {
            Debug.LogWarning($"Seulement {spawnedCount}/{count} médicaments ont pu être spawné (pas assez de spawn points)");
        }
    }

    //! ---------- Consommation des médicaments ----------

    public void OnMedicineEatenByPlayer(GameObject medicine) //& Appellé quand un médicament est mangé par le joueur
    {
        if (spawnedMedicines.Contains(medicine))
        {
            spawnedMedicines.Remove(medicine);
            medicinesNotEaten--;

            Debug.Log($"Médicament mangé ! Restants dans la scène: {medicinesNotEaten}");
            OnMedicineEaten?.Invoke(medicinesNotEaten);
        }
    }

    //! ---------- Stockage pour le jour suivant ----------

    public void StoreRemainingMedicines() //& Stocker les médicaments non mangés (en fonction de limite)
    {
        int toStore = Mathf.Min(medicinesNotEaten, maxStoredMedicines);

        if (toStore > 0)
        {
            storedMedicinesFromPreviousDay = toStore;
            Debug.Log($"{toStore} médicament(s) non mangé(s) seront disponibles demain (max: {maxStoredMedicines})");
        }
        else
        {
            storedMedicinesFromPreviousDay = 0;
            Debug.Log("Aucun médicament à stocker pour demain");
        }

        // Les médicaments excédentaires sont perdus
        if (medicinesNotEaten > maxStoredMedicines)
        {
            int lost = medicinesNotEaten - maxStoredMedicines;
            Debug.LogWarning($"{lost} médicament(s) perdu(s) (limite de stockage dépassée)");
        }
    }


    private void CleanupRemainingMedicines() //& Nettoie les médicaments restants dans la scènes
    {
        foreach (GameObject medicine in spawnedMedicines)
        {
            if (medicine != null)
                Destroy(medicine);
        }
        spawnedMedicines.Clear();
    }
    
    public void CleanupForDayRestart() //& Nettoie pour un relancement de jour (après une perte)
    {
        // Détruire tous les médicaments spawned
        CleanupRemainingMedicines();
        
        // Réinitialiser les compteurs
        medicinesNotEaten = 0;
        storedMedicinesFromPreviousDay = 0; // On perd tout quand on perd un jour
        
        Debug.Log("Médicaments réinitialisés pour restart du jour");
    }

    //! ---------- Getters ----------

    public int GetRemainingMedicines()
    {
        return medicinesNotEaten;
    }

    public int GetStoredMedicines()
    {
        return storedMedicinesFromPreviousDay;
    }

    public int GetMaxStoredMedicines()
    {
        return maxStoredMedicines;
    }
}
