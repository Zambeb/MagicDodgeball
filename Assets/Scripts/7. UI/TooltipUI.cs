using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    [SerializeField] private GameObject tooltipObject;
    [SerializeField] private TextMeshProUGUI tooltipText;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    private void Update()
    {
        // Следим за курсором
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent.GetComponent<RectTransform>(),
            Input.mousePosition,
            null,
            out position
        );
        transform.localPosition = position;
    }

    public void Show(string text)
    {
        tooltipText.text = text;
        tooltipObject.SetActive(true);
    }

    public void Hide()
    {
        tooltipObject.SetActive(false);
    }
}