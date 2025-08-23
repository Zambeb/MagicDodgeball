using UnityEngine;
using UnityEngine.InputSystem.UI;

public class SharedCanvas : MonoBehaviour
{
    [SerializeField] private Canvas sharedCanvas;
    [SerializeField] private MultiplayerEventSystem[] playerEventSystems;

    void Start()
    {
        foreach (var es in playerEventSystems)
        {
            es.playerRoot = sharedCanvas.gameObject;
        }
    }
}