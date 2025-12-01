using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_ObjekGameProperties", menuName = "Game/ObjekGameProperties")]
public class SO_ObjekGameProperties : ScriptableObject
{
    public long id;
    public string objekName;
    public int objekLevel;
    public string description;
    public ItemData dropItem;
    public int dropAmount;
    public float durabilty;
    public ItemToolType requiredTool;

}
