using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/**
 * Singleton UI pour le mini-jeu Arrow.
 * Initialisé une seule fois dans la scène, partagé par toutes les instances de S_ArrowMinigame.
 *
 * @since	v0.0.1
 * @version	v1.0.0	Saturday, February 28th, 2026.
 * @global
 */
public class S_ArrowMinigameUI : MonoBehaviour
{
    public static S_ArrowMinigameUI instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Transform sequenceContainer;
    [SerializeField] private Transform inputContainer;
    [SerializeField] private GameObject arrowImagePrefab;

    [Header("Arrow Sprites")]
    [SerializeField] private Sprite arrowUpSprite;
    [SerializeField] private Sprite arrowRightSprite;
    [SerializeField] private Sprite arrowDownSprite;
    [SerializeField] private Sprite arrowLeftSprite;

    [Header("Colors")]
    private Color normalColor = Color.white;
    private Color highlightColor = Color.green;
    private Color errorColor = Color.red;
    private Color emptyColor = new Color(1, 1, 1, 0.3f);

    private List<Image> sequenceArrows = new List<Image>();
    private List<Image> inputArrows = new List<Image>();

    private void Awake()
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

    /**
     * Affiche le panel du mini-jeu.
     *
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, February 28th, 2026.
     * @access	public
     * @return	void
     */
    public void Show()
    {
        minigamePanel.SetActive(true);
    }

    /**
     * Cache le panel du mini-jeu.
     *
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, February 28th, 2026.
     * @access	public
     * @return	void
     */
    public void Hide()
    {
        minigamePanel.SetActive(false);
    }

    /**
     * Met à jour le texte du timer.
     *
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, February 28th, 2026.
     * @access	public
     * @param	float	timeRemaining	
     * @return	void
     */
    public void UpdateTimer(float timeRemaining)
    {
        timerText.text = Mathf.Ceil(timeRemaining).ToString() + "s";
    }

    /**
     * Génère les éléments UI (flèches séquence + cases input) pour une longueur donnée.
     *
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, February 28th, 2026.
     * @access	public
     * @param	int	sequenceLength	
     * @return	void
     */
    public void GenerateArrowsUI(int sequenceLength)
    {
        ClearArrowsUI();

        for (int i = 0; i < sequenceLength; i++)
        {
            GameObject arrowObj = Instantiate(arrowImagePrefab, sequenceContainer);
            Image arrowImg = arrowObj.GetComponent<Image>();
            sequenceArrows.Add(arrowImg);
        }

        for (int i = 0; i < sequenceLength; i++)
        {
            GameObject arrowObj = Instantiate(arrowImagePrefab, inputContainer);
            Image arrowImg = arrowObj.GetComponent<Image>();
            inputArrows.Add(arrowImg);
        }
    }

    /**
     * Nettoie toutes les flèches UI existantes.
     *
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, February 28th, 2026.
     * @access	public
     * @return	void
     */
    public void ClearArrowsUI()
    {
        foreach (var arrow in sequenceArrows)
        {
            if (arrow != null) Destroy(arrow.gameObject);
        }
        foreach (var arrow in inputArrows)
        {
            if (arrow != null) Destroy(arrow.gameObject);
        }

        sequenceArrows.Clear();
        inputArrows.Clear();
    }

    /**
     * Affiche la séquence de flèches à reproduire.
     *
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, February 28th, 2026.
     * @access	public
     * @param	mixed	sequence	
     * @return	void
     */
    public void DisplaySequence(List<int> sequence)
    {
        for (int i = 0; i < sequenceArrows.Count && i < sequence.Count; i++)
        {
            sequenceArrows[i].sprite = GetArrowSprite(sequence[i]);
            sequenceArrows[i].color = normalColor;
            sequenceArrows[i].gameObject.SetActive(true);
        }
    }

    /**
     * Réinitialise les cases d'input (vides et transparentes).
     *
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, February 28th, 2026.
     * @access	public
     * @return	void
     */
    public void ResetInputDisplay()
    {
        foreach (var inputArrow in inputArrows)
        {
            inputArrow.color = emptyColor;
            inputArrow.sprite = null;
            inputArrow.gameObject.SetActive(true);
        }
    }

    /**
     * Marque une case d'input comme correcte (flèche verte).
     *
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, February 28th, 2026.
     * @access	public
     * @param	int	index    	
     * @param	int	direction	
     * @return	void
     */
    public void MarkInputCorrect(int index, int direction)
    {
        if (index < inputArrows.Count)
        {
            inputArrows[index].sprite = GetArrowSprite(direction);
            inputArrows[index].color = highlightColor;
        }
    }

    /**
     * Marque la flèche de séquence comme validée (verte).
     *
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, February 28th, 2026.
     * @access	public
     * @param	int	index	
     * @return	void
     */
    public void MarkSequenceValidated(int index)
    {
        if (index < sequenceArrows.Count)
        {
            sequenceArrows[index].color = highlightColor;
        }
    }

    /**
     * Marque une case d'input comme erreur (flèche rouge).
     *
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, February 28th, 2026.
     * @access	public
     * @param	int	index    	
     * @param	int	direction	
     * @return	void
     */
    public void MarkInputError(int index, int direction)
    {
        if (index < inputArrows.Count)
        {
            inputArrows[index].sprite = GetArrowSprite(direction);
            inputArrows[index].color = errorColor;
        }
    }

    /**
     * Retourne le sprite correspondant à une direction.
     *
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, February 28th, 2026.
     * @access	public
     * @param	int	direction	
     * @return	void
     */
    public Sprite GetArrowSprite(int direction)
    {
        switch (direction)
        {
            case 0: return arrowUpSprite;
            case 1: return arrowRightSprite;
            case 2: return arrowDownSprite;
            case 3: return arrowLeftSprite;
            default: return null;
        }
    }
}
