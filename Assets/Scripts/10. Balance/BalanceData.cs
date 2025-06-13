using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class BalanceData : MonoBehaviour
{
    private PlayerController _player1;
    private PlayerController _player2;
    
    [SerializeField] private TMP_Text player1Buffs;
    [SerializeField] private TMP_Text player2Buffs;
    [SerializeField] private TMP_Text player1Stats;
    [SerializeField] private TMP_Text player2Stats;
    [SerializeField] private TMP_Text coverageAndSurvivabilityText;

    public float fieldW;
    public float fieldL;
    private float fieldSize;
    
    public float player1Coverage;
    public float player2Coverage;
    public float player1InvincibilityTime;
    public float player2InvincibilityTime;
    public float player1Survivability;
    public float player2Survivability;

    public void Awake()
    {
        fieldSize = fieldW * fieldL;
    }

    public void UpdateAllData()
    {
        _player1 = RoundManager.Instance.player1;
        _player2 = RoundManager.Instance.player2;
        UpdateBuffsDisplay(_player1, _player2);
        UpdateStatsDisplay(_player1, _player2);
        player1Coverage = GetProjectileCoveragePercent(_player1, fieldSize);
        player2Coverage = GetProjectileCoveragePercent(_player2, fieldSize);
        player1InvincibilityTime = CalculateInvincibilityTime(_player1, 30);
        player2InvincibilityTime = CalculateInvincibilityTime(_player2, 30);
        player1Survivability = EstimateSurvivability(_player1, 30, player2Coverage, player1InvincibilityTime);
        player2Survivability = EstimateSurvivability(_player2, 30, player1Coverage, player2InvincibilityTime);
        
        coverageAndSurvivabilityText.text =
            $"P1 survivability: {player1Survivability:F2}\n" +
            $"P1 invincibility time: {30-player1InvincibilityTime:F2} seconds\n" +
            $"P1 coverage/second: {player1Coverage:F2}%\n\n" +
            $"P2 survivability: {player2Survivability:F2}\n" +
            $"P2 invincibility time: {30-player2InvincibilityTime:F2} seconds\n" +
            $"P2 coverage/second: {player2Coverage:F2}%";
    }
    private void UpdateBuffsDisplay(PlayerController player1, PlayerController player2)
    {
        if (player1 != null)
            player1Buffs.text = FormatBuffsList(player1.acquiredUpgradeEffectsPrefabs);
        
        if (player2 != null)
            player2Buffs.text = FormatBuffsList(player2.acquiredUpgradeEffectsPrefabs);
    }
    
    private string FormatBuffsList(List<UpgradeEffectBase> buffs)
    {
        if (buffs == null || buffs.Count == 0)
            return "No buffs";
        
        StringBuilder sb = new StringBuilder();
        
        foreach (var buff in buffs)
        {
            sb.AppendLine(buff.name); 
        }
        
        return sb.ToString();
    }

    private void UpdateStatsDisplay(PlayerController player1, PlayerController player2)
    {
        if (player1 != null)
            player1Stats.text = FormatStatsList(player1);
        
        if (player2 != null)
            player2Stats.text = FormatStatsList(player2);
    }

    private string FormatStatsList(PlayerController player)
    {
        StringBuilder sb = new StringBuilder();
        
        sb.AppendLine("PLAYER STATS:");
        sb.AppendLine($"Size: {player.transform.localScale.ToString("F2")}");
        sb.AppendLine($"Speed: {player.stats.moveSpeed.ToString("F2")}");
        sb.AppendLine($"Invincibility %: ");
        
        sb.AppendLine("\nPROJECTILE STATS:");
        sb.AppendLine($"Size: {(player.stats.ballSizeMultiplier * 0.5f).ToString("F2")}");
        sb.AppendLine($"Quantity: {player.stats.maxProjectiles}");
        sb.AppendLine($"Bounces: {player.stats.maxBounces}");
        sb.AppendLine($"Acceleration multiplier: {player.stats.accelerationAfterBounce.ToString("F2")}");
        sb.AppendLine($"Initial speed: {player.stats.projectileSpeed.ToString("F2")}");
        
        float maxSpeed = player.stats.projectileSpeed;
        if(player.stats.maxBounces > 0 && player.stats.accelerationAfterBounce > 1f)
        {
            maxSpeed *= Mathf.Pow(player.stats.accelerationAfterBounce, player.stats.maxBounces);
        }
        sb.AppendLine($"Max speed: {maxSpeed.ToString("F2")}");
    
        return sb.ToString();
    }
    
    float GetProjectileCoveragePercent(PlayerController player, float fieldArea)
    {
        int count = player.stats.maxProjectiles;
        int bounces = player.stats.maxBounces;
        float radius = 0.25f * player.stats.ballSizeMultiplier; 
        float baseSpeed = player.stats.projectileSpeed;
        float accel = player.stats.accelerationAfterBounce;
        
        float maxSpeed = baseSpeed;
        if (bounces > 0 && accel > 1f)
            maxSpeed *= Mathf.Pow(accel, bounces);
        
        float averageSpeed = (baseSpeed + maxSpeed) / 2f;
        
        float coveragePerProjectile = 2f * radius * averageSpeed;
        float totalCoverage = count * coveragePerProjectile;
        
        float coveragePercent = (totalCoverage / fieldArea) * 100f;
        return coveragePercent;
    }

    float EstimateSurvivability(PlayerController player, float roundDuration, float threatCoveragePercent, float invincibilityTime)
    {
        float speed = player.stats.moveSpeed;
        float size = player.transform.localScale.magnitude;
        
        float mobilityScore = speed / size;

        int shieldCount = 0;
        foreach (var buff in player.acquiredUpgradeEffectsPrefabs)
        {
            if (buff.name == "Shield") shieldCount++;
        }

        float shieldFactor = Mathf.Clamp01(shieldCount / 12f); 
        float threatFactor = Mathf.Clamp01(threatCoveragePercent / 100f); 
        float invincibilityFactor = Mathf.Clamp01(invincibilityTime / roundDuration);
        float resistance = 1f - threatFactor;   
        float survivabilityRaw = mobilityScore * (1 + invincibilityFactor + shieldFactor) * resistance;

        float normalized = Mathf.Clamp01(survivabilityRaw / 2f); 
        return normalized * 100f;
    }

    float CalculateInvincibilityTime(PlayerController player, float roundDuration)
    {
        float invincibilityTime = 0;
        
        if (player.acquiredActiveAbility != null)
        {
            if (player.acquiredActiveAbility.name == "Dash")
            {
                invincibilityTime += (0.4f * 6f);
            }

            else if (player.acquiredActiveAbility.name == "Force Field")
            {
                invincibilityTime += (2f * 3f);
            }
        }

        invincibilityTime += Mathf.Min(roundDuration, roundDuration / player.stats.immunityAfterHit);
        
        return invincibilityTime;
    }
}
