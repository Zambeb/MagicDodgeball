using UnityEngine;
using UnityEngine.UI;

public class VictoryDisplayUI : MonoBehaviour
{
    [SerializeField] private Image[] player1Crystals;
    [SerializeField] private Image[] player2Crystals;

    [SerializeField] private Sprite crystalGrey1;
    [SerializeField] private Sprite crystalGrey2;
    [SerializeField] private Sprite crystalBlue;
    [SerializeField] private Sprite crystalRed;

    public void UpdateVictoryCrystals(int p1Wins, int p2Wins)
    {
        UpdatePlayer1Crystals(player1Crystals, p1Wins, crystalBlue);
        UpdatePlayer2Crystals(player2Crystals, p2Wins, crystalRed);
    }

    private void UpdatePlayer1Crystals(Image[] crystals, int wins, Sprite coloredSprite)
    {
        for (int i = 0; i < crystals.Length; i++)
        {
            if (i < wins)
            {
                crystals[i].sprite = coloredSprite;
            }
            else
            {
                crystals[i].sprite = crystalGrey1;
            }
        }
    }
    
    private void UpdatePlayer2Crystals(Image[] crystals, int wins, Sprite coloredSprite)
    {
        for (int i = 0; i < crystals.Length; i++)
        {
            if (i < wins)
            {
                crystals[i].sprite = coloredSprite;
            }
            else
            {
                crystals[i].sprite = crystalGrey2;
            }
        }
    }
}