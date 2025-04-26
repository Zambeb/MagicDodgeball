using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Upgrades/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    [TextArea]
    public string description;
    
    public UpgradeEffectBase effectPrefab;

    public UpgradeEffectBase CreateEffect()
    {
        return Instantiate(effectPrefab);
    }
}
