using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private GameObject audioManager;

    private void Start()
    {
        // Find Audio
        if (FindObjectOfType<AudioManager>() == null)
        {
            Instantiate(audioManager, transform.position, Quaternion.identity);
        }

        FindObjectOfType<AudioManager>().Play("BGM");
    }

    public void PlayButton()
    {
        SceneManager.LoadScene(1);
        //loadingPanel.SetActive(true);
        //StartCoroutine(LoadAsynchoronously(1));
    }
    IEnumerator LoadAsynchoronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            // Calculate progress
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            // Change slider
            loadingSlider.value = progress;
            // Return null
            yield return null;
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
