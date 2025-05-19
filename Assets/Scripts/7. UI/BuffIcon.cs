using UnityEngine;
using UnityEngine.EventSystems;

public class BuffIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string tooltipText;

    public void SetTooltip(string text)
    {
        tooltipText = text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Instance.Show(tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }
}
