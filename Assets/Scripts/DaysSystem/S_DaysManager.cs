using System;
using UnityEngine;

public class S_DaysManager : MonoBehaviour
{
    //! S_DaysManager gère le gameplay général, le changement de jour etc...
    public static S_DaysManager instance { get; private set; }

    //~ Information du système de jours
    [SerializeField] private float dayDuration = 300f; // Durée d'une journée en seconde
    [SerializeField] private float percentageLucidityJaugeAward = 15; // Pourcentage récupérer de jauge de lucidité en pourcentage
    [SerializeField] private int maxDays = 15; // Jours max pour atteindre la fin du jeu

    //~ Information du jour actuel
    private int currentDay = 1; // Jour actuel par défaut 1
    private float timeLasted = 0; // Temps ecoulé actuellement
    private bool isDayActive = false; // Jour actif ou non

    //~ Actions
    public event Action OnDayEnd;
    public event Action OnDayLost; // Event quand le joueur perd un jour

    //TODO Manque la gestion des quetes du jour

    void Awake() //& Création du manager
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

        StartDay(); // TEST

    }

    void Update() //& Gère l'écoulement du jour
    {
        if (isDayActive)
        {
            HandleTime();
        }
    }

    //! ---------- Gestion du temps ----------

    private void HandleTime() //& Ecoulement du temps
    {
        timeLasted += Time.deltaTime;

        // Vérification de si le jour est terminé et trigger l'event de fin
        if (timeLasted >= dayDuration)
        {
            EndDay();
        }
    }

    private void EndDay() //& Fin du jour
    {
        isDayActive = false;
        OnDayEnd?.Invoke(); // Lance l'event de fin de jour

        Debug.Log($"Jour {currentDay} terminé après {timeLasted:F2} secondes");

        Award(); // Récompense le joueur

        // Vérification de si on peut passer au jour suivant
        if (currentDay < maxDays)
        {
            PrepareNextDay();
        }
        else
        {
            Debug.Log("Fin du jeu atteinte ! Victoire !");
            //TODO Fin du jeu (victoire)
        }
    }

    private void Award() //& Récompense du joueur
    {
        //TODO Récompense de lucidité via S_LucidityManager
        Debug.Log($"Récompense de {percentageLucidityJaugeAward}% de lucidité");
    }

    private void PrepareNextDay() //& Prépare le jour d'après, gènère tout ce qu'il faut
    {
        timeLasted = 0f;
        SetCurrentDay(currentDay + 1);

        Debug.Log($"Préparation du jour {currentDay}");
        //TODO Générer les quêtes, randomiser le soleil, etc.

        RandomizeSunTime(); // Randomiser le soleil entre 10h et 18h

        // On commence le prochain jour
        StartDay();
    }

    //! ---------- Randomisation du soleil ----------

    private void RandomizeSunTime() //& Randomise l'heure du soleil entre 10h et 18h
    {
        // Convertir 10h et 18h en temps normalisé (0..1)
        // 10h = 10/24 = 0.4167
        // 18h = 18/24 = 0.75
        float minTime = 10f / 24f; // 10h
        float maxTime = 18f / 24f; // 18h

        float randomTime = UnityEngine.Random.Range(minTime, maxTime);

        S_DayNightManager.instance.SetTime(randomTime);

        // Afficher l'heure choisie dans les logs
        string timeString = S_DayNightManager.instance.GetTimeString(randomTime);
        Debug.Log($"Soleil randomisé à {timeString}");
    }

    //! ---------- Système de perte de jour ----------

    /* Explication 
       Perd 1 jour si:
            - Jauge de lucidité à 0
            - Fin du jour sans avoir effectué toutes les quetes
     */
    public void LoseDay(string reason) //& Perte du jour
    {
        isDayActive = false;

        Debug.LogWarning($"Jour perdu ! Raison: {reason}");

        OnDayLost?.Invoke(); // Lance l'event de perte de jour

        // Si on est au jour 1, on recommence le jour 1
        if (currentDay == 1)
        {
            Debug.Log("Recommencement du jour 1");
            timeLasted = 0f;
            //TODO Réinitialiser les quêtes, la lucidité, etc.
        }
        else
        {
            // Sinon on recule d'un jour
            SetCurrentDay(currentDay - 1);
            timeLasted = 0f;
            Debug.Log($"Retour au jour {currentDay}");
            //TODO Réinitialiser les quêtes du jour actuel
        }

        //TODO Afficher un écran de transition/feedback au joueur
    }

    public void OnLucidityReachedZero() //& Appellé quand la jauge de lucidité atteint 0
    {
        LoseDay("Jauge de lucidité à 0");
    }

    public void OnMainQuestsNotCompleted() //& Appellé quand les quetes principales incompletes
    {
        LoseDay("Quêtes principales non complétées");
    }

    //! ---------- Méthodes publiques ----------

    public void StartDay()
    {
        timeLasted = 0f;
        isDayActive = true;
        Debug.Log($"Jour {currentDay} commencé");
    }

    public void PauseDay()
    {
        isDayActive = false;
    }

    public void ResumeDay()
    {
        isDayActive = true;
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }

    public float GetDayProgress()
    {
        return Mathf.Clamp01(timeLasted / dayDuration);
    }

    public float GetTimeRemaining()
    {
        return Mathf.Max(0, dayDuration - timeLasted);
    }

    private void SetCurrentDay(int newDay)
    {
        // Gestion des erreurs
        if (newDay == currentDay)
        {
            Debug.LogWarning("Erreur: NewDay équivaut à CurrentDay");
            return;
        }
        else if (newDay > maxDays)
        {
            Debug.LogWarning("Erreur: NewDay dépasse la limite MaxDays");
            return;
        }
        else if (newDay < 1)
        {
            Debug.LogWarning("Erreur: NewDay ne peut pas être inférieur à 1");
            return;
        }

        currentDay = newDay;
    }
}
