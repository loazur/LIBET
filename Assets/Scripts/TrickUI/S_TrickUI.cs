

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/**
 * Système d'astuces (Tips) qui affiche des popups aléatoires à intervalles réguliers.
 * Supporte le multi-langues (FR/EN).
 * 
 * Setup dans Unity:
 *   - Placer ce script sur un GameObject dans le Canvas
 *   - Créer un enfant "PopupPanel" avec un CanvasGroup et un TextMeshProUGUI
 *   - Assigner les références dans l'Inspector
 *   - Remplir les listes TrickName_FR et TrickName_EN avec les mêmes indices
 *
 * @author  Lucas
 * @since   v0.0.1
 * @version v1.0.0  Friday, February 28th, 2026.
 */
public class S_TrickUI : MonoBehaviour
{
    public static S_TrickUI instance { get; private set; }

    [Header("Textes d'astuces")]
    [Tooltip("Liste de phrases en français (même ordre que la liste EN)")]
    [SerializeField] private List<string> TrickName_FR = new List<string>();

    [Tooltip("Liste de phrases en anglais (même ordre que la liste FR)")]
    [SerializeField] private List<string> TrickName_EN = new List<string>();

    [Header("Timing")]
    [Tooltip("Durée d'affichage du popup (secondes)")]
    [SerializeField] private float displayDuration = 7f;

    [Tooltip("Intervalle entre chaque popup (secondes)")]
    [SerializeField] private float intervalBetweenTips = 60f;

    [Tooltip("Délai avant le premier popup (secondes)")]
    [SerializeField] private float initialDelay = 10f;

    [Header("Animation")]
    [Tooltip("Durée du fade in (secondes)")]
    [SerializeField] private float fadeInDuration = 0.4f;

    [Tooltip("Durée du fade out (secondes)")]
    [SerializeField] private float fadeOutDuration = 0.4f;

    [Tooltip("Distance de slide depuis le bas (pixels)")]
    [SerializeField] private float slideDistance = 50f;

    [Header("UI References")]
    [Tooltip("CanvasGroup du popup (gère l'alpha et l'interactivité)")]
    [SerializeField] private CanvasGroup popupCanvasGroup;

    [Tooltip("RectTransform du popup (pour l'animation de slide)")]
    [SerializeField] private RectTransform popupRectTransform;

    [Tooltip("Texte du popup")]
    [SerializeField] private TextMeshProUGUI tipText;

    private Coroutine tipLoopCoroutine;
    private Coroutine animationCoroutine;
    private Vector2 popupBasePosition;
    private List<int> availableIndices = new List<int>();
    private bool isEnabled = true;

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

    void Start()
    {
        // Sauvegarder la position de base AVANT de cacher
        if (popupRectTransform != null)
        {
            popupBasePosition = popupRectTransform.anchoredPosition;
        }

        // Cacher complètement le popup au démarrage
        HidePopupImmediate();

        // Réinitialiser la liste des indices disponibles
        ResetAvailableIndices();

        // Lancer la boucle d'astuces
        tipLoopCoroutine = StartCoroutine(TipLoop());
    }

    void OnDestroy()
    {
        if (tipLoopCoroutine != null)
            StopCoroutine(tipLoopCoroutine);

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
    }

    #region Boucle principale

    /// <summary>
    /// Boucle principale : attend, affiche un tip, attend, recommence.
    /// </summary>
    private IEnumerator TipLoop()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            if (isEnabled && GetCurrentTipList().Count > 0)
            {
                ShowRandomTip();
            }

            yield return new WaitForSeconds(intervalBetweenTips);
        }
    }

    #endregion

    #region Affichage des astuces

    /// <summary>
    /// Affiche une astuce aléatoire avec animation.
    /// Utilise un système de shuffle pour éviter les répétitions.
    /// </summary>
    public void ShowRandomTip()
    {
        List<string> tips = GetCurrentTipList();
        if (tips.Count == 0) return;

        // Si tous les tips ont été affichés, réinitialiser
        if (availableIndices.Count == 0)
        {
            ResetAvailableIndices();
        }

        // Piocher un index aléatoire sans répétition
        int randomPick = UnityEngine.Random.Range(0, availableIndices.Count);
        int tipIndex = availableIndices[randomPick];
        availableIndices.RemoveAt(randomPick);

        // Sécurité : vérifier que l'index est valide
        if (tipIndex >= tips.Count)
        {
            ResetAvailableIndices();
            return;
        }

        string tip = tips[tipIndex];

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(ShowTipAnimation(tip));
    }

    /// <summary>
    /// Affiche un tip spécifique par index (utile pour forcer un tip précis).
    /// </summary>
    public void ShowTipByIndex(int index)
    {
        List<string> tips = GetCurrentTipList();
        if (index < 0 || index >= tips.Count) return;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(ShowTipAnimation(tips[index]));
    }

    /// <summary>
    /// Force le masquage immédiat du popup.
    /// </summary>
    public void HideTip()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        HidePopupImmediate();
    }

    /// <summary>
    /// Cache le popup instantanément (alpha 0 + SetActive false).
    /// </summary>
    private void HidePopupImmediate()
    {
        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.alpha = 0f;
            popupCanvasGroup.blocksRaycasts = false;
            popupCanvasGroup.interactable = false;
        }

        if (popupRectTransform != null)
        {
            popupRectTransform.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Animations

    /// <summary>
    /// Animation complète : activer → slide-in + fade-in → attente → fade-out + slide-out → désactiver.
    /// </summary>
    private IEnumerator ShowTipAnimation(string tip)
    {
        if (popupCanvasGroup == null || popupRectTransform == null || tipText == null)
            yield break;

        // Préparer le texte avant d'activer
        tipText.text = tip;

        // Activer le panel (invisible, alpha 0)
        popupCanvasGroup.alpha = 0f;
        popupRectTransform.gameObject.SetActive(true);

        // --- Fade In + Slide Up ---
        yield return StartCoroutine(FadeIn());

        // --- Attendre la durée d'affichage ---
        yield return new WaitForSeconds(displayDuration);

        // --- Fade Out + Slide Down ---
        yield return StartCoroutine(FadeOut());

        // Désactiver complètement le panel
        popupRectTransform.gameObject.SetActive(false);
    }

    /// <summary>
    /// Fade in avec slide vers le haut.
    /// </summary>
    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Vector2 startPos = popupBasePosition + Vector2.down * slideDistance;
        Vector2 endPos = popupBasePosition;

        popupCanvasGroup.alpha = 0f;
        popupRectTransform.anchoredPosition = startPos;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / fadeInDuration);

            popupCanvasGroup.alpha = t;
            popupRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        popupCanvasGroup.alpha = 1f;
        popupRectTransform.anchoredPosition = endPos;
        popupCanvasGroup.blocksRaycasts = false;
        popupCanvasGroup.interactable = false;
    }

    /// <summary>
    /// Fade out avec slide vers le bas.
    /// </summary>
    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Vector2 startPos = popupBasePosition;
        Vector2 endPos = popupBasePosition + Vector2.down * slideDistance;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / fadeOutDuration);

            popupCanvasGroup.alpha = 1f - t;
            popupRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        popupCanvasGroup.alpha = 0f;
        popupRectTransform.anchoredPosition = popupBasePosition;
        popupCanvasGroup.blocksRaycasts = false;
        popupCanvasGroup.interactable = false;
    }

    #endregion

    #region Utilitaires

    /// <summary>
    /// Retourne la liste de tips selon la langue courante.
    /// </summary>
    private List<string> GetCurrentTipList()
    {
        if (S_GameUserData.instance != null &&
            S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            return TrickName_EN;
        }

        return TrickName_FR;
    }

    /// <summary>
    /// Réinitialise la liste d'indices disponibles (shuffle bag).
    /// </summary>
    private void ResetAvailableIndices()
    {
        availableIndices.Clear();
        int count = Mathf.Max(TrickName_FR.Count, TrickName_EN.Count);
        for (int i = 0; i < count; i++)
        {
            availableIndices.Add(i);
        }
    }

    /// <summary>
    /// Active ou désactive le système d'astuces.
    /// </summary>
    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;

        if (!enabled)
        {
            HideTip();
        }
    }

    /// <summary>
    /// Redémarre le timer (utile après un changement de scène ou un event).
    /// </summary>
    public void RestartLoop()
    {
        if (tipLoopCoroutine != null)
            StopCoroutine(tipLoopCoroutine);

        tipLoopCoroutine = StartCoroutine(TipLoop());
    }

    #endregion
}