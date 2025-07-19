using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    //public GameObject loadingScreenPanel;

    [Header("Gameplay")]
    public GameScene gameSceneToLoad = GameScene.ArenaSquare;
    
    [Header("Name Inputs")]
    public TMP_InputField player1InputField;
    public TMP_InputField player2InputField;

    public void PlayGame()
    {
        SoundManager.Instance.PlaySFX("Click");
        GameManager.Instance.LoadSceneAsync(gameSceneToLoad, mainMenuPanel);
    }

    public void LoadSquareArena()
    {
        SoundManager.Instance.PlaySFX("Click");
        GameManager.Instance.LoadSceneAsync(GameScene.ArenaSquare, mainMenuPanel);
    }
    
    public void LoadOctagonArena()
    {
        SoundManager.Instance.PlaySFX("Click");
        GameManager.Instance.LoadSceneAsync(GameScene.ArenaOctagon, mainMenuPanel);
    }
    
    public void LoadSnowArena()
    {
        SoundManager.Instance.PlaySFX("Click");
        GameManager.Instance.LoadSceneAsync(GameScene.ArenaSnow, mainMenuPanel);
    }

    public void OpenSettings()
    {
        SoundManager.Instance.PlaySFX("Click");
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        SoundManager.Instance.PlaySFX("Click");
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        SoundManager.Instance.PlaySFX("Click");
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        SoundManager.Instance.PlaySFX("Click");
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void SetPlayer1Name(string enteredName)
    {
        GameManager.Instance.player1Name = enteredName;
    }
    
    public void SetPlayer2Name(string enteredName)
    {
        GameManager.Instance.player2Name = enteredName;
    }
}