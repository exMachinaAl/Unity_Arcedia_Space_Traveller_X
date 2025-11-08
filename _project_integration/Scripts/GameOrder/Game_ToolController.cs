using UnityEngine;

public enum ToolType
{
    None,
    Hand,
    Pickaxe,
    Laser,
	Axe,
    // tambah sesuai kebutuhan
}

public class Game_ToolController : MonoBehaviour
{
    // ini contoh: kamu bisa ganti mekanik equip sesuai sistemmu (UI / hotbar)
    public ToolType currentTool = ToolType.Hand;

    // public API
    public bool IsTool(ToolType t) => currentTool == t;
}
