using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScaler : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Vector3 scaledUp;
    private float duration = 0.5f; // Время анимации
    private float t = 0f;
    private bool scaling = false;

    void Start()
    {
        originalScale = transform.localScale;
        scaledUp = originalScale * 1.1f;
        targetScale = originalScale;
    }

    void Update()
    {
        if (scaling)
        {
            t += Time.unscaledDeltaTime / duration;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, t);

            // когда доходим почти до цели — останавливаем
            if (Vector3.Distance(transform.localScale, targetScale) < 0.001f)
            {
                transform.localScale = targetScale;
                scaling = false;
            }
        }
    }

    private void ScaleTo(Vector3 newScale)
    {
        targetScale = newScale;
        t = 0f;
        scaling = true;
    }

    public void OnSelect(BaseEventData eventData) => ScaleTo(scaledUp);
    public void OnDeselect(BaseEventData eventData) => ScaleTo(originalScale);
    public void OnPointerEnter(PointerEventData eventData) => ScaleTo(scaledUp);
    public void OnPointerExit(PointerEventData eventData) => ScaleTo(originalScale);
}