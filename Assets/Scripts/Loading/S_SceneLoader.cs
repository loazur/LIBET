using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_SceneLoader : MonoBehaviour
{
    //! S_SceneLoader permet de charger la scène de manière asynchrone avec un chargement visuel
    public static S_SceneLoader instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider slider;

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

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        loadingScreen.SetActive(true);

        while(!operation.isDone) // Tant que le chargement charge
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;

            yield return null;
        }
    }
}
