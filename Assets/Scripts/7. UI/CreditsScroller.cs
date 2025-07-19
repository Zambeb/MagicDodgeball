using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditsScroller : MonoBehaviour
{
    public RectTransform creditsPanel;         // Панель с текстом титров
    public float scrollSpeed = 50f;            // Скорость прокрутки

    public GameObject creditsCanvas;           // Канва титров
    public GameObject mainMenuCanvas;          // Канва главного меню

    private float startY;
    private float endY;
    private bool isScrolling = false;

    void Update()
    {
        if (!isScrolling) return;

        creditsPanel.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (creditsPanel.anchoredPosition.y >= endY)
        {
            // Повтор титров
            RestartCredits();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainMenu();
        }
    }

    public void StartCredits()
    {
        creditsCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);

        // Начинаем прямо снизу экрана — Pos Y = 0 (pivot = 1)
        startY = 0f;
        creditsPanel.anchoredPosition = new Vector2(creditsPanel.anchoredPosition.x, startY);

        // Конец — это высота титров + немного запаса (например, экран)
        endY = creditsPanel.rect.height + 100f; // запас для полного ухода за экран

        isScrolling = true;
    }

    public void RestartCredits()
    {
        creditsPanel.anchoredPosition = new Vector2(creditsPanel.anchoredPosition.x, startY);
    }

    public void ReturnToMainMenu()
    {
        isScrolling = false;
        creditsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
}