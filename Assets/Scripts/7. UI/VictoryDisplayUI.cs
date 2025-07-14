using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private GameObject vfxPrefab;
    
    [SerializeField] private Transform crystalOverlayContainer; 
    [SerializeField] private Canvas mainCanvas;

    private List<int> roundWinners = new(); // 0 = Player 1, 1 = Player 2

    public void RegisterRoundWinner(int playerIndex)
    {
        roundWinners.Add(playerIndex); 
    }

    public void ShowVictoryDisplay(int p1Wins, int p2Wins)
    {
        StartCoroutine(AnimateLastCrystal(p1Wins, p2Wins));
    }

    private IEnumerator AnimateLastCrystal(int p1Wins, int p2Wins)
    {
        int lastWinner = roundWinners.Count > 0 ? roundWinners[^1] : -1;
        if (lastWinner == -1) yield break;

        Image[] targetCrystals = lastWinner == 0 ? player1Crystals : player2Crystals;
        Sprite colorSprite = lastWinner == 0 ? crystalBlue : crystalRed;
        Sprite greySprite = lastWinner == 0 ? crystalGrey1 : crystalGrey2;

        int crystalIndex = lastWinner == 0 ? p1Wins - 1 : p2Wins - 1;
        if (crystalIndex < 0 || crystalIndex >= targetCrystals.Length) yield break;

        Image crystal = targetCrystals[crystalIndex];
        crystal.enabled = false;
        
        Vector3 worldPos = crystal.rectTransform.position;
        
        GameObject overlayObj = new GameObject("CrystalOverlay");
        overlayObj.transform.SetParent(crystalOverlayContainer, false); 

        Image overlay = overlayObj.AddComponent<Image>();
        overlay.sprite = colorSprite;
        overlay.color = new Color(1f, 1f, 1f, 0f);
        overlay.preserveAspect = true;

        RectTransform overlayRect = overlay.rectTransform;
        overlayRect.position = worldPos;
        overlayRect.sizeDelta = crystal.rectTransform.sizeDelta;
        overlayRect.localScale = Vector3.one * 4f;

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            overlay.rectTransform.localScale = Vector3.Lerp(Vector3.one * 4f, Vector3.one, t * t);
            float alpha = 0.5f + 0.5f * t;
            overlay.color = new Color(1f, 1f, 1f, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        overlay.rectTransform.localScale = Vector3.one;
        overlay.color = Color.white;

        // Spawn effect
        if (vfxPrefab != null)
        {
            GameObject vfx = Instantiate(vfxPrefab, overlayObj.transform.position, Quaternion.identity, crystalOverlayContainer);
            UIManager.Instance.ShakeUpgradeScreen(0.5f, 20f, 25f);
            
            FeelManager.Instance.rumble.FadeOutRumble(1f, 1f, 1f);
            
            Destroy(vfx, 2f);
        }
        
        yield return new WaitForSeconds(0.2f);

        crystal.enabled = true;
        crystal.sprite = colorSprite;

        Destroy(overlayObj);
    }

    public void ResetDisplay()
    {
        roundWinners.Clear();

        foreach (var c in player1Crystals) c.sprite = crystalGrey1;
        foreach (var c in player2Crystals) c.sprite = crystalGrey2;
    }
}