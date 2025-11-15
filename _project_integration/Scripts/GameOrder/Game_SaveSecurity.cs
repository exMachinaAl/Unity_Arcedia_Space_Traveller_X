using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SaveSecurity
{
    private static string secretKey = "SUPER_SECRET_KEY_123"; // obfuscate nanti

    // public static string GenerateChecksum(PlayerSave data)
    // {
    //     if (data == null) return null;

    //     StringBuilder sb = new StringBuilder();

    //     // Base player info (safe null handling)
    //     sb.Append(data.playerName ?? "");
    //     sb.Append("|");
    //     sb.Append(data.scienceCredit);
    //     sb.Append("|");
    //     sb.Append(data.lastWorld.ToString() ?? "");
    //     sb.Append("|");

    //     // Planet data hash
    // //    foreach (var p in data.planetsInterrupted)
    //     // {
    //     //     var planetBuilder = new StringBuilder();

    //     //     planetBuilder.Append(p.planetId ?? "");
    //     //     planetBuilder.Append(":");

    //     //     foreach (var nodeId in p.depletedNodes)
    //     //     {
    //     //         planetBuilder.Append(nodeId ?? "");
    //     //         planetBuilder.Append(","); // pemisah
    //     //     }

    //     //     // Buang koma terakhir jika ada
    //     //     if (p.depletedNodes.Count > 0)
    //     //         planetBuilder.Length--;

    //     //     sb.Append(planetBuilder.ToString());
    //     //     sb.Append("|");
    //     // }

    //     foreach (var p in data.planetsInterrupted)
    //     {
    //         var nodeString = string.Join(",", p.depletedNodes);
    //         sb.Append($"{p.planetId}:{nodeString}|");
    //     }


    //     // Secret key at end
    //     sb.Append(secretKey);

    //     string content = sb.ToString();

    //     using (var sha = SHA256.Create())
    //     {
    //         byte[] input = Encoding.UTF8.GetBytes(content);
    //         byte[] hash = sha.ComputeHash(input);
    //         return BitConverter.ToString(hash).Replace("-", "");
    //     }
    // }

    public static string GenerateChecksum(PlayerSave data)
    {
        if (data == null) return null;

        StringBuilder sb = new StringBuilder();

        sb.Append(data.playerId ?? "");
        sb.Append("|");
        sb.Append(data.playerName ?? "");
        sb.Append("|");
        sb.Append(data.universeSeed);
        sb.Append("|");
        sb.Append(data.galaxySeed);
        sb.Append("|");
        sb.Append(data.playerMode);
        sb.Append("|");
        sb.Append(data.playerInThe);
        sb.Append("|");
        sb.Append(data.scienceCredit);
        sb.Append("|");
        sb.Append(data.lastWorld);
        sb.Append("|");

        // Sort planets for deterministic order
        data.planetsInterrupted.Sort((a, b) => a.planetId.CompareTo(b.planetId));

        foreach (var p in data.planetsInterrupted)
        {
            p.depletedNodes.Sort();

            sb.Append(p.planetId);
            sb.Append(":");
            sb.Append(string.Join(",", p.depletedNodes));
            sb.Append("|");
        }

        // HMAC style final string
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
        {
            byte[] input = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] hash = hmac.ComputeHash(input);
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }


    public static bool ValidateChecksum(PlayerSave data)
    {
        if (data == null || string.IsNullOrEmpty(data.checksum)) return false;

        string calculated = GenerateChecksum(data);
        return string.Equals(calculated, data.checksum, StringComparison.OrdinalIgnoreCase);
    }
}
