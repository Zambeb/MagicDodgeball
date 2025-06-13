using UnityEngine;
using UnityEngine.InputSystem;

public class CheatMenuManager : MonoBehaviour
{
    [Header("Cheat Menu Panel")]
    [SerializeField] private GameObject cheatMainPanel;

    private bool isPaused = false;
    private bool panelOpen = false;

    void Update()
    {
        if (Keyboard.current.backquoteKey.wasPressedThisFrame)
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        panelOpen = !panelOpen;
        cheatMainPanel.SetActive(panelOpen);

        if (panelOpen)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Game Resumed");
    }

    public void EndRound()
    {
        RoundManager.Instance.TryEndRound();
    }
}