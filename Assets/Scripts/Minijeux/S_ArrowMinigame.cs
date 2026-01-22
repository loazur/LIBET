using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_ArrowMinigame : S_AbstractMinigame
{
    private GameObject minigame;
    [SerializeField] private Image background;
    [SerializeField] private Image arrow1;
    [SerializeField] private Image arrow2;
    [SerializeField] private Image arrow3;
    [SerializeField] private Image arrow4;

    [SerializeField] private List<Image> arrows = new List<Image>(); // up, right, down, left

    private List<int> sequence;

    /* FERMER
        if (S_MenuManager.instance != null) 
        {
            S_MenuManager.instance.RegisterMenuClose(S_MenuManager.MenuType.MINIGAME);
        }
        */

    public override void TriggerMinigame()
    {
        Debug.Log("Minijeu commencé!");

        // Lancer un menu
        if (S_MenuManager.instance != null)
        {
            if (!S_MenuManager.instance.RegisterMenuOpen(S_MenuManager.MenuType.MINIGAME))
            {
                Debug.LogWarning("[ArrowMinigame] Impossible de démarrer le menu ArrowMinigame, un menu est ouvert");
                return;
            }
        }

        //TODO

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        minigame.SetActive(false);

        //initialize sequence randomly:
        sequence = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            sequence.Add(Random.Range(0, 4)); // assuming 4 different arrows
            switch (sequence[i])
            {
                case 0:
                    // up
                    // arrow_images[i].sprite = arrow1;
                    break;
                case 1:
                    // right
                    // arrow_images[i].sprite = arrow2;
                    break;
                case 2:
                    // down
                    // arrow_images[i].sprite = arrow3;
                    break;
                case 3:
                    // left
                    // arrow_images[i].sprite = arrow4;
                    break;
            }
        }



        // subscribe to the event that triggers the minigame
    }

}
