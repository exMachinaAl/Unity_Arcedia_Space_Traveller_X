using UnityEngine;

public class MobileInput : MonoBehaviour, IPlayerInput {
    // public VirtualJoystick moveJoystick;
    // public VirtualJoystick lookJoystick;

    private bool interactBtn;
    private bool inventoryBtn;
    private bool questBtn;
    private bool JumpBtn;
    public void OnInteractButton() => interactBtn = true;
    public void OnInventoryButton() => inventoryBtn = true;
    public void OnQuestButton() => questBtn = true;
    public void OnJumpButton() => JumpBtn = true;
    public void OnSprintButtonClicked()
    {
        SprintToggle = !SprintToggle;
    }


    // public Vector2 MoveAxis => moveJoystick.Axis;
    // public Vector2 LookAxis => lookJoystick.Axis;
    public Vector2 MoveAxis => new Vector2(0, 0);
    public Vector2 LookAxis => new Vector2(0,0);

    public bool SprintToggle { get; private set; }
    public bool Jump { get { var t = JumpBtn; JumpBtn = false; return t; } }
    // public bool Jump => false; // Atur sesuai UI
    public bool Interact { get { var t = interactBtn; interactBtn = false; return t; } }
    public bool OpenInventory { get { var t = inventoryBtn; inventoryBtn = false; return t; } }
    public bool OpenQuest { get { var t = questBtn; questBtn = false; return t; } }
}
