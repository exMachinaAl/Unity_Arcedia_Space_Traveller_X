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
public class WorldSession
{
    public long planetId;
    public long planetSeed;
    public float surfaceAtmosRadius;
    public float planetVisualRadius;
    public float AtmosphereRadius;
    public float atmosphereHeight;
    public Vector3 planetCenter;
    public Vector3 entryNormal;
    public Vector3 entryPosition;

    // ---- Helper stable formatting ----
    private string VecToString(Vector3 v)
    {
        return $"{v.x:F6}|{v.y:F6}|{v.z:F6}";
    }

    // ---- Public helpers if needed individually ----
    public string PlCenterToString()       => VecToString(planetCenter);
    public string PlEntryNormalToString()  => VecToString(entryNormal);
    public string PlEntryPositionToString() => VecToString(entryPosition);

    // ---- Full checksum builder for this class ----
    public string ToChecksumString()
    {
        return $"{planetId}|" +
               $"{planetSeed}|" +
               $"{surfaceAtmosRadius:F6}|" +
               $"{planetVisualRadius:F6}|" +
               $"{AtmosphereRadius:F6}|" +
               $"{atmosphereHeight:F6}|" +
               $"{VecToString(planetCenter)}|" +
               $"{VecToString(entryNormal)}|" +
               $"{VecToString(entryPosition)}";
    }
}

[System.Serializable]
public class PlayerSave
{
    public string playerId;
    public string playerName;
    public Inventory playerInventory;
    public Vector3 playerPosition;
    public List<Inventory> remotePlayerInventory = new List<Inventory>();
    public int universeSeed;
    public long galaxySeed;
    public PlayerMode playerMode;
    public PlayerInThe playerInThe;
    public int scienceCredit;
    public List<PlanetInvatedData> planetsInterrupted = new List<PlanetInvatedData>();
    public long lastWorldId;
    public long lastWorldSeed;
    public WorldSession worldSession;
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
        if (Game_SaveSystem.Instance == null && Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); Load(); }
        else Destroy(gameObject);
    }
    private void OnApplicationQuit()
    {
        SetPlayerPosition();
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
        // Fungsi untuk menyimpan data ke penyimpanan (misalnya file)
    public void SaveInventory(Game_PlayerInventory playerInventory)
    {
        // Menyinkronkan data dari PlayerInventory ke SaveManager
        save.playerInventory.items.Clear(); // Bersihkan stack lama
        save.playerInventory.items.AddRange(playerInventory.inventory.items); // Salin data

        Save();
        Debug.Log("Inventory saved!");
    }

    // Fungsi untuk memuat data dari penyimpanan
    public void LoadInventory(Game_PlayerInventory playerInventory)
    {
        // Menyinkronkan data dari SaveManager ke PlayerInventory
        playerInventory.inventory.items.Clear();

        if (save.playerInventory == null)
        {
            Debug.Log("No inventory data found in save.");
            return;
        }

        playerInventory.inventory.items.AddRange(save.playerInventory.items);
        
        Debug.Log("Inventory loaded!");
    }
    public void SetWorldSession(long planetId, long planetSeed, float surfaceRds, float planetVisRds, float AtmosphereRadius, float atmosphereHeight, Vector3 planetCenter, Vector3 entryNrl, Vector3 entryPos)
    {
        save.worldSession.planetId = planetId;
        save.worldSession.planetSeed = planetSeed;
        save.worldSession.surfaceAtmosRadius = surfaceRds;
        save.worldSession.planetVisualRadius = planetVisRds;
        save.worldSession.AtmosphereRadius = AtmosphereRadius;
        save.worldSession.atmosphereHeight = atmosphereHeight;
        save.worldSession.planetCenter = planetCenter;
        save.worldSession.entryNormal = entryNrl;
        save.worldSession.entryPosition = entryPos;
        Save();
        
    }
    public void SetPlayerPosition()
    {
        if (Manager_Player.Instance == null)
        {
            save.playerPosition = new Vector3(0, 0, 0);
            Logger.LogWarning("Save manager", "Manager_Player Instance is null, setting player position to (0,0,0)");
            Save();
            return;
        }

        // var currPlayerMode = GetPlayerMode();
        if (Manager_Player.Instance.flightCtrl != null && Manager_Player.Instance.flightCtrl.shipTransform != null && GetPlayerMode() == PlayerMode.Flight)
            save.playerPosition = Manager_Player.Instance.flightCtrl.shipTransform.position;

        if (Manager_Player.Instance.humanCtrl != null && Manager_Player.Instance.humanCtrl.transform != null && GetPlayerMode() == PlayerMode.Human)
            save.playerPosition = Manager_Player.Instance.humanCtrl.transform.position;

        Save();
    }
    public Vector3 GetPlayerPosition()
    {
        return save.playerPosition;
    }
    public void SetPlayerMode(PlayerMode SetMode)
    {
        save.playerMode = SetMode;
        Save();
    }
    public PlayerMode GetPlayerMode()
    {
        return save.playerMode;
    }
    public void SetPlayerInWorld(PlayerInThe currStateW)
    {
        save.playerInThe = currStateW;
        Save();
    }
    public PlayerInThe GetPlayerInStateWorld()
    {
        return save.playerInThe;
    }
    public void SetCurrentPlanetId(long planetId)
    {
        save.lastWorldId = planetId;
        Save();
    }
    public long GetCurrentPlanetId() => save.lastWorldId; 
    public long GetCurrentPlanetSeed() => save.lastWorldSeed;

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
        save.playerPosition = new Vector3(0, 7, 0);
        save.universeSeed = 1361640601; // set custom seedUniverse
        // save.universeSeed = rng.NextInt(1, int.MaxValue);
        save.galaxySeed = SeedUtil.SubSeed((long)save.universeSeed, 0);
        save.playerMode = PlayerMode.Human; // ini debug fast space loh ya, kalo new game
        // save.playerMode = PlayerMode.Human;
        save.playerInThe = PlayerInThe.Space;
        // save.playerInThe = PlayerInThe.Ground;
        save.scienceCredit = 0;
        // save.lastWorld = SeedUtil.SubSeed(save.galaxySeed, 0);
        save.lastWorldId = SeedUtil.MakePlanetId((int)save.galaxySeed, 0, 0, 0); //broken, karena seharusnya dibuat murni statik
        save.lastWorldSeed = SeedUtil.SurfaceSeedV3(save.galaxySeed, 0);
        // save.world

        // Game_SaveSystem.Instance.SetWorldSession( // semenstara debug
        //     loadPlanetData.planetId,
        //     loadPlanetData.planetSeed,
        //     loadPlanetData.surfaceAtmosRadius,
        //     loadPlanetData.planetVisualRadius,
        //     loadPlanetData.AtmosphereRadius,
        //     loadPlanetData.atmosphereHeight,
        //     loadPlanetData.planetCenter,
        //     loadPlanetData.entryNormal,
        //     player.position
        // );

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

        Debug.Log("✅ Save Data OK. Last world: " + save.lastWorldId + " Seed: " + save.lastWorldSeed);
    }

}
