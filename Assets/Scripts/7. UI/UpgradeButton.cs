using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private UpgradeData upgradeData;
    private UpgradeScreen upgradeScreen;

    public void Setup(UpgradeData data, UpgradeScreen screen)
    {
        upgradeData = data;
        upgradeScreen = screen;

        iconImage.sprite = data.icon;
        nameText.text = data.upgradeName;
        descriptionText.text = data.description;
    }

    public void OnClick()
    {
        upgradeScreen.SelectUpgrade(upgradeData);
    }
}