using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

[System.Serializable]
public class PlanetInvatedData
{
    public long planetId;
    public List<long> depletedNodes = new List<long>();
}

[System.Serializable]
public class PlayerSave
{
    public string playerId;
    public string playerName;
    public int universeSeed;
    public long galaxySeed;
    public PlayerMode playerMode;
    public PlayerInThe playerInThe;
    public int scienceCredit;
    public List<PlanetInvatedData> planetsInterrupted = new List<PlanetInvatedData>();
    public long lastWorld;
    public string checksum;
}

public class Game_SaveSystem : MonoBehaviour
{
    public static Game_SaveSystem Instance { get; private set; }
    public string path => Path.Combine(Application.persistentDataPath, "/_Project_Of_Now/A_Unity_/Arcedia_Space_Traveller_X/Arcedia_Space_Traveller_X_URP/Assets/_project_integration/SaveData/save.json");
    // public string path => Path.Combine(Application.persistentDataPath, "/save.json");
    public string secretKey = "YourSecretKey123"; // bisa kamu ganti
    public PlayerSave save = new PlayerSave();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); Load(); }
        else Destroy(gameObject);
    }

    public void MarkNodeDepleted(long planetId, long objekResourceId)
    {
        var planetData = save.planetsInterrupted.Find(p => p.planetId == planetId);
        if (planetData == null)
        {
            planetData = new PlanetInvatedData { planetId = planetId };
            save.planetsInterrupted.Add(planetData);
        }
        if (!planetData.depletedNodes.Contains(objekResourceId))
        {
            planetData.depletedNodes.Add(objekResourceId);
            Save();
        }
    }
    public bool IsNodeDepleted(long planetId, long nodeId)
    {
        var planetData = save.planetsInterrupted.Find(p => p.planetId == planetId);
        if (planetData == null) return false;
        return planetData.depletedNodes.Contains(nodeId);
    }
    public void setCurrentPlanetId(long planetId)
    {
        save.lastWorld = planetId;
        Save();
    }
    public long getCurrentPlanetId() 
    {
        return save.lastWorld; 
    }

    public bool setNameByCutscene(string name)
    { 
        save.playerName = name;
        Save();
        return true;
    }
    public bool setNewGame()
    {
        uint seed = (uint)System.DateTime.Now.Ticks;
        Unity.Mathematics.Random rng = new Unity.Mathematics.Random(seed);

        save.playerId = Guid.NewGuid().ToString();
        save.playerName = "none";
        save.universeSeed = rng.NextInt(1, int.MaxValue);
        save.galaxySeed = SeedUtil.SubSeed((long)save.universeSeed, 0);
        save.playerMode = PlayerMode.Flight;
        save.playerInThe = PlayerInThe.Space;
        // save.playerMode = PlayerMode.Human;
        // save.playerInThe = PlayerInThe.Ground;
        save.scienceCredit = 0;
        save.lastWorld = SeedUtil.SubSeed(save.galaxySeed, 0);

        if (Root_GameStartManager.isDebugMode)
        {
            save.scienceCredit = 1000;
            save.playerName = "Mod";
        }

        Save();
        return true;
    }
    public PlayerSave getFullSaveData()
    {
        return save;
    } 


    public void Save()
    {
        // kosongkan checksum dulu
        save.checksum = "";

        // generate checksum tanpa checksum field
        string newChecksum = SaveSecurity.GenerateChecksum(save);
        save.checksum = newChecksum;

        // serialize
        string json = JsonUtility.ToJson(save, true);
        File.WriteAllText(path, json);

        Debug.Log("✅ Saved with checksum: " + save.checksum);
    }


    public void Load()
    {
        if (!File.Exists(path))
        {
            Debug.Log("🆕 No save found");
            save = new PlayerSave();
            return;
        }

        string json = File.ReadAllText(path);
        PlayerSave data = JsonUtility.FromJson<PlayerSave>(json) ?? new PlayerSave();

        // backup checksum yg disimpan
        string fileChecksum = data.checksum;

        // kosongkan sebelum cek
        data.checksum = "";

        string calculated = SaveSecurity.GenerateChecksum(data);

        if (fileChecksum != calculated)
        {
            Debug.LogWarning("⚠️ Save file corrupted / edited!");
            save = new PlayerSave(); // atau jangan overwrite?
            return;
        }

        // restore real data + simpan checksum original
        data.checksum = fileChecksum;
        save = data;

        Debug.Log("✅ Save Data OK. Last world: " + save.lastWorld);
    }

}
