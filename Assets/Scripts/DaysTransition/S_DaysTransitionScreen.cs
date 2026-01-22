using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_DaysTransitionScreen : MonoBehaviour
{
    public static S_DaysTransitionScreen instance;

    [Header("Information de l'UI")]
    [SerializeField] private GameObject uiContainer;
    [SerializeField] private TextMeshProUGUI textDay;
    [SerializeField] private TextMeshProUGUI textJauge;
    [SerializeField] private TextMeshProUGUI textMedicines;
    [SerializeField] private TextMeshProUGUI textLore;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup fadeCanvasGroup; // Panel noir pour le fade
    [SerializeField] private float fadeDuration = 1f; // Durée du fondu

    public event Action OnTransitionScreenEnd;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //!---------------------------------------

    public void TriggerTransitionScreen(int day, float jauge, int medicines, string[] lore, float duration)
    {
        // Notifier le manager qu'un menu s'ouvre
        if (S_MenuManager.instance != null)
        {
            if (!S_MenuManager.instance.RegisterMenuOpen(S_MenuManager.MenuType.DAYS_TRANSITION))
            {
                Debug.LogWarning("[DAYS_TRANSITION] Impossible de démarrer le menu Days_Transition, un menu est ouvert");
                return;
            }
        }

        // Affichage des informations en fonction de la langue
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French) // Français
        {
            textDay.text = $"Jour {day}";
            textJauge.text = $"Etat jauge : {jauge}";
            textMedicines.text = $"Medicaments stockes : {medicines}";

            textLore.text = lore[0];
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English) // Anglais
        {
            textDay.text = $"Day {day}";
            textJauge.text = $"Jauge state : {jauge}";
            textMedicines.text = $"Stored medicines : {medicines}";

            textLore.text = lore[1];
        }

        uiContainer.SetActive(true);

        // Coroutine de transition
        StartCoroutine(TransitionSequence(duration));
    }

    private IEnumerator TransitionSequence(float displayDuration)
    {
        // Fade in (noir vers transparent)
        yield return StartCoroutine(FadeIn());

        // Attendre la durée d'affichage
        yield return new WaitForSeconds(displayDuration);

        // Fade out (transparent vers noir)
        yield return StartCoroutine(FadeOut());

        // Fermer l'écran
        if (S_MenuManager.instance != null)
        {
            S_MenuManager.instance.RegisterMenuClose(S_MenuManager.MenuType.DAYS_TRANSITION);
        }

        
        uiContainer.SetActive(false);

        OnTransitionScreenEnd?.Invoke();
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        fadeCanvasGroup.alpha = 1f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = 1f - (elapsed / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        fadeCanvasGroup.alpha = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = elapsed / fadeDuration;
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }

}
