// PlayerUpgradeLogger.cs
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerUpgradeLogger : MonoBehaviour
{
    private string sessionFilePath;
    private List<string> roundLogs = new List<string>();

    public void InitLogSession()
    {
        string folderPath = Path.Combine(Application.dataPath, "UpgradeLogs");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        sessionFilePath = Path.Combine(folderPath, $"GameSession_{timestamp}.csv");

        // Write header
        File.WriteAllText(sessionFilePath, "Round,Player1Upgrades,Player2Upgrades,Winner\n");
    }

    public void LogRound(int roundNumber, PlayerController player1, PlayerController player2, int winnerIndex)
    {
        string p1Upgrades = UpgradesToString(player1);
        string p2Upgrades = UpgradesToString(player2);

        string winner = winnerIndex == -1 ? "Draw" : $"Player{winnerIndex + 1}";

        string line = $"Round {roundNumber}:\nPlayer 1 upgrades: {p1Upgrades}\nPlayer 2 upgrades: {p2Upgrades}\nWinner: {winner}\n\n";
        roundLogs.Add(line);
        File.AppendAllText(sessionFilePath, line);
    }

    private string UpgradesToString(PlayerController player)
    {
        List<string> upgradeNames = new List<string>();
        foreach (var upgrade in player.acquiredUpgrades)
        {
            upgradeNames.Add(upgrade.name);
        }

        if (player.acquiredActiveAbility != null)
        {
            upgradeNames.Add($"Active:{player.acquiredActiveAbility.name}");
        }

        return string.Join("|", upgradeNames);
    }

    public void FinalizeLog()
    {
        File.AppendAllText(sessionFilePath, $"\nTotal Rounds: {roundLogs.Count}\n");
    }
}