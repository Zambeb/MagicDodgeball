using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Upgrades/Data/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public LocalizedString upgradeName;
    public Sprite icon;
    public LocalizedString descriptionIfKeyboard;
    public LocalizedString descriptionIfGamepad;
    
    public UpgradeEffectBase effectPrefab;

    public UpgradeEffectBase CreateEffect()
    {
        return Instantiate(effectPrefab);
    }
}
