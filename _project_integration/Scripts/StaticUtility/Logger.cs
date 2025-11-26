using UnityEngine;

public static class Logger
{
    public static void LogNormal(string tag, string message)
    {
        Debug.Log($"[ {tag} ] pesan = {message}");
    }
    public static void LogWarning(string tag, string message)
    {
        Debug.LogWarning($"[ {tag} ] pesan = {message}");
    }
    public static void LogError(string tag, string message)
    {
        Debug.LogError($"[ {tag} ] pesan = {message}");
    }
}