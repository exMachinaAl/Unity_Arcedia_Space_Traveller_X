using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class MineralExData
{
    public string mineralId;
    public float spawnChance;  // probabilitas mineral ini ada di planet
    public float richness;     // seberapa banyak dia kalau muncul
    public float variability;

    public override string ToString()
    {
        return $"MineralExData(id: {mineralId}, spawnChance: {spawnChance}, richness: {richness}, variability: {variability})";
    }
}

[CreateAssetMenu(fileName = "RNG_PlanetData", menuName = "randomness/PlanetData")]
public class RNG_PlanetData : ScriptableObject
{

    

    // public MineralExData[] mineralData;
    public List<MineralExData> mineralResources = new List<MineralExData>();


    //     public string questTitle;
    //     public string questDescription;

    //     [Header("Requirement")]
    //     public string targetItemName;
    //     public int targetItemCount;

    //     [Header("Completion Mode")]
    //     public QuestCompletionMode completionMode = QuestCompletionMode.AutoComplete;

    //     [Tooltip("NPC name or ID to return to")]
    //     public string returnToNPC;
}
