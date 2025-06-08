using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [SerializeField] private GameObject pauseMenuUI;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
        pauseMenuUI.SetActive(true);
        RoundManager.Instance.player1.DisableCharacter(); ;
        RoundManager.Instance.player2.DisableCharacter();
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuUI.SetActive(false);
        RoundManager.Instance.player1.EnableCharacter();
        RoundManager.Instance.player2.EnableCharacter();
    }

    public bool IsPaused => isPaused;
}