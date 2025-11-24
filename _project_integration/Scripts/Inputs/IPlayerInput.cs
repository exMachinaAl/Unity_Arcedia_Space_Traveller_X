using UnityEngine;

public interface IPlayerInput
{
    Vector2 MoveAxis { get; }
    Vector2 LookAxis { get; }
    bool SprintToggle { get; }
    bool Jump { get; }
    bool Interact { get; }
    bool OpenInventory { get; }
    bool OpenQuest { get; }
}
