using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private List<SceneEntry> scenes = new();

    private Dictionary<GameScene, string> sceneDict;
    
    [Header("UI Panels")]
    [SerializeField] private GameObject loadingScreenPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSceneDict();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeSceneDict()
    {
        sceneDict = new Dictionary<GameScene, string>();
        foreach (var entry in scenes)
        {
            if (!sceneDict.ContainsKey(entry.sceneType))
            {
                sceneDict.Add(entry.sceneType, entry.sceneName);
            }
        }
    }

    public void LoadScene(GameScene scene)
    {
        if (sceneDict.TryGetValue(scene, out string sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene name for {scene} not found in GameManager.");
        }
    }
    
    public string GetSceneName(GameScene scene)
    {
        if (sceneDict != null && sceneDict.TryGetValue(scene, out string sceneName))
        {
            return sceneName;
        }
        Debug.LogError($"Scene name for {scene} not found in GameManager.");
        return null;
    }
    
    public void LoadSceneAsync(GameScene scene, GameObject menuToHide = null)
    {
        if (sceneDict.TryGetValue(scene, out string sceneName))
        {
            StartCoroutine(LoadSceneCoroutine(sceneName, menuToHide));
        }
        else
        {
            Debug.LogError($"Scene name for {scene} not found in GameManager.");
        }
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, GameObject menuToHide)
    {
        if (menuToHide != null)
            menuToHide.SetActive(false);

        if (loadingScreenPanel != null)
            loadingScreenPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }

        if (loadingScreenPanel != null)
            loadingScreenPanel.SetActive(false);
    }
    
    public void QuitGame()
    {
        SoundManager.Instance.PlaySFX("Click");
        Debug.Log("Quit Game");
        Application.Quit();
    }
}

