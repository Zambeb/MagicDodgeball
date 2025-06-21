using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Upgrades/Data/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    [TextArea(3, 10)] 
    public string descriptionIfKeyboard;
    [TextArea(3, 10)] 
    public string descriptionIfGamepad;
    
    public UpgradeEffectBase effectPrefab;

    public UpgradeEffectBase CreateEffect()
    {
        return Instantiate(effectPrefab);
    }
}
