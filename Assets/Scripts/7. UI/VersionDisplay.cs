using TMPro;
using UnityEngine;

public class VersionDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionText;

    private static VersionDisplay instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
            return;
        }

        if (versionText != null)
        {
            versionText.text = "v" + Application.version;
        }
    }
}
