using UnityEngine;
using UnityEngine.UI;

public class VictoryDisplayUI : MonoBehaviour
{
    [SerializeField] private Image[] player1Crystals;
    [SerializeField] private Image[] player2Crystals;

    [SerializeField] private Sprite crystalGrey;
    [SerializeField] private Sprite crystalBlue;
    [SerializeField] private Sprite crystalRed;

    public void UpdateVictoryCrystals(int p1Wins, int p2Wins)
    {
        UpdatePlayerCrystals(player1Crystals, p1Wins, crystalBlue);
        UpdatePlayerCrystals(player2Crystals, p2Wins, crystalRed);
    }

    private void UpdatePlayerCrystals(Image[] crystals, int wins, Sprite coloredSprite)
    {
        for (int i = 0; i < crystals.Length; i++)
        {
            if (i < wins)
            {
                crystals[i].sprite = coloredSprite;
            }
            else
            {
                crystals[i].sprite = crystalGrey;
            }
        }
    }
}