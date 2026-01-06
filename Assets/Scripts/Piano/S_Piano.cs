using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class S_Piano : MonoBehaviour, SI_Interactable
{
    [Header("Piano Keys Settings")]
    [SerializeField] private float timePressed = 0.2f;
    [SerializeField] private int numberOfKeysSimulated = 5;
    [SerializeField] private float playInterval = 0.5f;
    [SerializeField] private float maxUseDistance = 2.5f;

    //! eviter de modifier cette valeur
    private float keyPressDepth = 0.022f; //! Important: doit correspondre au déplacement visuel des touches
    
    private List<GameObject> pianoKeys = new();

    private bool isPlaying = false;
    private Coroutine playRoutine;
    private Transform currentPlayer;

    private string interactText = "Jouer du piano";

    void Start()
    {
        UpdateInteractText();

        foreach (Transform child in transform)
            if (child.name.StartsWith("touche"))
                pianoKeys.Add(child.gameObject);
    }

    void Update()
    {
        if (!isPlaying || currentPlayer == null) return;

        float distance = Vector3.Distance(transform.position, currentPlayer.position);
        if (distance > maxUseDistance)
            StopPlaying();
    }

    // ===================== INTERACTION =====================

    /**
     * Toggle le jeu du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	public
     * @param	transform	playerTransform	
     * @return	void
     */
    public void Interact(Transform playerTransform)
    {
        if (isPlaying)
            StopPlaying();
        else
            StartPlaying(playerTransform);
    }

    /**
     * Fonctions de gestion du jeu du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @param	transform	player	
     * @return	void
     */
    private void StartPlaying(Transform player)
    {
        isPlaying = true;
        currentPlayer = player;
        playRoutine = StartCoroutine(PlayLoop());
    }

    /**
     * Arrête le jeu du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @return	void
     */
    private void StopPlaying()
    {
        if (!isPlaying) return;

        isPlaying = false;
        currentPlayer = null;

        if (playRoutine != null)
            StopCoroutine(playRoutine);
    }

    // ===================== PLAY LOOP =====================

    /**
     * Coroutine de simulation du jeu du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @return	void
     */
    private IEnumerator PlayLoop()
    {
        while (isPlaying)
        {
            MoovAllTouch();
            yield return new WaitForSeconds(playInterval);
        }
    }

    // ===================== TOUCHES =====================

    /**
     * Simule l'appui sur plusieurs touches du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @return	void
     */
    private void MoovAllTouch()
    {
        List<GameObject> selectedKeys = SelectRandomKeys();

        foreach (GameObject key in selectedKeys)
            MoveOneTouch(key);

        StartCoroutine(ResetKeysAfterDelay(selectedKeys, timePressed));
    }

    /**
     * Réinitialise les touches après un délai
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @param	mixed	keys 	
     * @param	float	delay	
     * @return	mixed
     */
    private IEnumerator ResetKeysAfterDelay(List<GameObject> keys, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject key in keys)
            ResetOneTouch(key);
    }

    /**
     * Bouger une touche du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @param	gameobject	key	
     * @return	void
     */
    private void MoveOneTouch(GameObject key)
    {
        key.transform.localPosition -= new Vector3(0, keyPressDepth, 0);
    }

    /**
     * Reset une touche du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @param	gameobject	key	
     * @return	void
     */
    private void ResetOneTouch(GameObject key)
    {
        key.transform.localPosition += new Vector3(0, keyPressDepth, 0);
    }

    // Sectionner des touches aléatoires
    private List<GameObject> SelectRandomKeys()
    {
        List<GameObject> selected = new();
        HashSet<int> used = new();

        while (selected.Count < numberOfKeysSimulated && selected.Count < pianoKeys.Count)
        {
            int i = Random.Range(0, pianoKeys.Count);
            if (used.Add(i))
                selected.Add(pianoKeys[i]);
        }

        return selected;
    }

    // ===================== UI =====================

    public string getInteractText()
    {
        return isPlaying ? "Arrêter de jouer" : interactText;
    }

    public Transform getTransform() => transform;

    private void UpdateInteractText() 
    {
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            interactText = "Jouer du piano";
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            interactText = "Play the piano";
        }
    }
}
