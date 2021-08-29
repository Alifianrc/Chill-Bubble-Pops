using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [HideInInspector] public static bool MenuIsActive;
    
    public void PauseButton()
    {
        MenuIsActive = true;
        menuPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeButton()
    {
        MenuIsActive = false;
        menuPanel.SetActive(false);
        Time.timeScale = 1;
    }
    
    public void ReturnToMenuButton()
    {
        FindObjectOfType<GameManager>().SaveHighScore();
        MenuIsActive = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
