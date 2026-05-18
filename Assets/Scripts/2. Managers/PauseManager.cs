using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject mainPauseMenu;
    [SerializeField] private GameObject balanceMenu;
    [SerializeField] private GameObject optionsMenu;
    
    [Header("UI Navigation")]
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject pauseMenuFirstButton; 
    [SerializeField] private GameObject optionsMenuFirstButton;
    [SerializeField] private GameObject balanceMenuFirstButton;

    private bool isPaused = false;
    
    private int currentPausingPlayer = -1;
    private GameObject previousPlayerRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Pause(int playerIndex)
    {
        if (isPaused) return;
        
        Time.timeScale = 0f;
        isPaused = true;
        currentPausingPlayer = playerIndex;
        
        RemovePlayerRootCage(playerIndex);
        
        DefaultPause();
        
        SetFocusForPlayer(playerIndex, pauseMenuFirstButton);
        
        if (RoundManager.Instance.roundActive)
        {
            RoundManager.Instance.player1.DisableCharacter();
            RoundManager.Instance.player2.DisableCharacter();
        }

        Cursor.visible = true;
    }

    public void Resume() 
    {
        Time.timeScale = 1f;
        isPaused = false;
        
        ClearFocusForPlayer(currentPausingPlayer);
        RestorePlayerRootCage(currentPausingPlayer);
        
        currentPausingPlayer = -1;
        
        pauseMenuUI.SetActive(false);

        if (RoundManager.Instance.roundActive)
        {
            RoundManager.Instance.player1.EnableCharacter();
            RoundManager.Instance.player2.EnableCharacter();
            Cursor.visible = false;
        }
    }

    private void DefaultPause()
    {
        pauseMenuUI.SetActive(true);
        BackToMainPauseMenu();
    }

    private void RemovePlayerRootCage(int playerIndex)
    {
        GameObject eventSystemObj = GameObject.Find($"EventSystems/UIInputModule_{playerIndex}");
        if (eventSystemObj != null)
        {
            var mpes = eventSystemObj.GetComponent<MultiplayerEventSystem>();
            if (mpes != null)
            {
                // Сохраняем личный UI игрока (меню способностей)
                previousPlayerRoot = mpes.playerRoot;
                
                // Назначаем корень на глобальное меню паузы! Теперь геймпад видит кнопки
                if (pauseMenuCanvas != null)
                {
                    mpes.playerRoot = pauseMenuCanvas; 
                }
            }
        }
    }

    private void RestorePlayerRootCage(int playerIndex)
    {
        if (playerIndex == -1 || previousPlayerRoot == null) return;
        
        GameObject eventSystemObj = GameObject.Find($"EventSystems/UIInputModule_{playerIndex}");
        if (eventSystemObj != null)
        {
            var mpes = eventSystemObj.GetComponent<MultiplayerEventSystem>();
            if (mpes != null)
            {
                mpes.playerRoot = previousPlayerRoot;
                previousPlayerRoot = null; 
            }
        }
    }

    private void SetFocusForPlayer(int playerIndex, GameObject targetButton)
    {
        GameObject eventSystemObj = GameObject.Find($"EventSystems/UIInputModule_{playerIndex}");
        if (eventSystemObj != null)
        {
            var mpes = eventSystemObj.GetComponent<MultiplayerEventSystem>();
            if (mpes != null)
            {
                mpes.SetSelectedGameObject(null);
                mpes.SetSelectedGameObject(targetButton);
            }
        }
    }

    private void ClearFocusForPlayer(int playerIndex)
    {
        if (playerIndex == -1) return;
        
        GameObject eventSystemObj = GameObject.Find($"EventSystems/UIInputModule_{playerIndex}");
        if (eventSystemObj != null)
        {
            var mpes = eventSystemObj.GetComponent<MultiplayerEventSystem>();
            if (mpes != null)
            {
                mpes.SetSelectedGameObject(null);
            }
        }
    }
    
    public void OpenBalanceMenu()
    {
        mainPauseMenu.SetActive(false);
        balanceMenu.SetActive(true);
        balanceMenu.GetComponent<BalanceData>().UpdateAllData();
        
        SetFocusForPlayer(currentPausingPlayer, balanceMenuFirstButton);
    }
    
    public void OpenOptionsMenu()
    {
        mainPauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
        
        SetFocusForPlayer(currentPausingPlayer, optionsMenuFirstButton);
    }

    public void BackToMainPauseMenu()
    {
        mainPauseMenu.SetActive(true);
        balanceMenu.SetActive(false);
        optionsMenu.SetActive(false);
        
        if (isPaused)
        {
            SetFocusForPlayer(currentPausingPlayer, pauseMenuFirstButton);
        }
    }

    public void MainMenu()
    {
        Resume();
        GameManager.Instance.LoadScene(GameScene.MainMenu);
    }
    
    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    public bool IsPaused => isPaused;
}