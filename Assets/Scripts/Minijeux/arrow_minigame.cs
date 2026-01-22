using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class arrow_minigame : MonoBehaviour
{

    private GameObject minigame;
    [SerializeField] private Image background;
    [SerializeField] private Image arrow1;
    [SerializeField] private Image arrow2;
    [SerializeField] private Image arrow3;
    [SerializeField] private Image arrow4;

    [SerializeField] private List<Image> arrows = new List<Image>(); // up, right, down, left

    private List<int> sequence;

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
