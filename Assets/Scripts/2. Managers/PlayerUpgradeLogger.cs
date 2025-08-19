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

    public void LogRound(int roundNumber, PlayerController player1, PlayerController player2, int winnerIndex, int p1Wins, int p2Wins)
    {
        LogPlayerUpgrades(roundNumber, 1, player1, winnerIndex == 0, p1Wins, p2Wins);
        LogPlayerUpgrades(roundNumber, 2, player2, winnerIndex == 1, p1Wins, p2Wins);
    }

    private void LogPlayerUpgrades(int round, int playerIndex, PlayerController player, bool isWinner, int p1Wins, int p2Wins)
    {
        foreach (var upgrade in player.acquiredUpgrades)
        {
            string line = $"{round},{playerIndex},{upgrade.name},Passive,{(isWinner ? 1 : 0)},{p1Wins},{p2Wins}\n";
            File.AppendAllText(sessionFilePath, line);
        }

        if (player.acquiredActiveAbility != null)
        {
            string line = $"{round},{playerIndex},{player.acquiredActiveAbility.name},Active,{(isWinner ? 1 : 0)},{p1Wins},{p2Wins}\n";
            File.AppendAllText(sessionFilePath, line);
        }
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