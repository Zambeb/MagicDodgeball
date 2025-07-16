using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject mainPauseMenu;
    [SerializeField] private GameObject balanceMenu;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
        DefaultPause();
        if (RoundManager.Instance.roundActive)
        {
            RoundManager.Instance.player1.DisableCharacter();
            RoundManager.Instance.player2.DisableCharacter();
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuUI.SetActive(false);
        if (RoundManager.Instance.roundActive)
        {
            RoundManager.Instance.player1.EnableCharacter();
            RoundManager.Instance.player2.EnableCharacter();
        }
    }

    private void DefaultPause()
    {
        pauseMenuUI.SetActive(true);
        BackToMainPauseMenu();
    }

    public void OpenBalanceMenu()
    {
        mainPauseMenu.SetActive(false);
        balanceMenu.SetActive(true);
        balanceMenu.GetComponent<BalanceData>().UpdateAllData();
    }

    public void BackToMainPauseMenu()
    {
        mainPauseMenu.SetActive(true);
        balanceMenu.SetActive(false);
    }

    public void MainMenu()
    {
        GameManager.Instance.LoadScene(GameScene.MainMenu);
    }
    
    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    public bool IsPaused => isPaused;
}