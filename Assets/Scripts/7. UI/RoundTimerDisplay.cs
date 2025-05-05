using UnityEngine;
using UnityEngine.UI; // Используй TMPro, если у тебя TextMeshPro
using TMPro;

public class RoundTimerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update()
    {
        if (RoundManager.Instance == null) return;

        float timeLeft = RoundManager.Instance.GetRemainingTime();
        timerText.text = Mathf.CeilToInt(timeLeft).ToString();
    }
}