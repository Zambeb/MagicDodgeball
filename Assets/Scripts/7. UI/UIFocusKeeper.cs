using UnityEngine;
using UnityEngine.EventSystems;

public class UIFocusKeeper : MonoBehaviour
{
    public EventSystem eventSystem;
    private GameObject lastSelected;

    void Start()
    {
        if (eventSystem == null)
            eventSystem = EventSystem.current;
            
        lastSelected = eventSystem.firstSelectedGameObject;
    }

    void Update()
    {
        // Если текущий выбранный объект существует, запоминаем его
        if (eventSystem.currentSelectedGameObject != null)
        {
            lastSelected = eventSystem.currentSelectedGameObject;
        }
        // Если фокус потерян (кликнули мышкой в пустоту)
        else
        {
            // Возвращаем фокус на последний выбранный элемент
            eventSystem.SetSelectedGameObject(lastSelected);
        }
    }
}