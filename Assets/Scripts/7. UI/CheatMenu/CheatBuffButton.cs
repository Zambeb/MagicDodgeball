using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheatBuffButton : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    private UpgradeData upgradeData;
    private CheatPlayerBuffs upgradeScreen;
    
    public void Setup(UpgradeData data, CheatPlayerBuffs screen)
    {
        upgradeData = data;
        upgradeScreen = screen;
        
        nameText.text = data.upgradeName;

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
}
