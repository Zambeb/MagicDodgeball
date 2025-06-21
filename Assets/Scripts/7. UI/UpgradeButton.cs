using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    //[SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    public UpgradeData upgradeData;
    private UpgradeScreen upgradeScreen;

    public void Setup(UpgradeData data, UpgradeScreen screen, PlayerController player)
    {
        upgradeData = data;
        upgradeScreen = screen;

        iconImage.sprite = data.icon;
        //nameText.text = data.upgradeName;
        if (player.currentControlScheme == "Gamepad")
        {
            descriptionText.text = data.descriptionIfGamepad;
        }
        else
        {
            descriptionText.text = data.descriptionIfKeyboard;
        }
        
        
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (upgradeData != null && upgradeScreen != null)
        {
            upgradeScreen.SelectUpgrade(upgradeData);
        }
        else if (upgradeData == null && upgradeScreen != null)
        {
            Debug.Log("No upgrade data!!!");
        }
        else if (upgradeData != null && upgradeScreen == null)
        {
            Debug.Log("No upgrade screen!!!");
        }
        else if (upgradeData == null && upgradeScreen == null)
        {
            Debug.Log("No upgrade data AND upgrade screen!!!");
        }
        else
        {
            Debug.Log("Something's totally wrong");
        }
    }

    public UpgradeData GetUpgradeData()
    {
        return upgradeData;
    }
}