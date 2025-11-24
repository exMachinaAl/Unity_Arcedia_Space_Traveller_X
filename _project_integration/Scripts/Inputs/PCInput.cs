using UnityEngine;

public class PCInput : MonoBehaviour, IPlayerInput {
    public Vector2 MoveAxis => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    public Vector2 LookAxis => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    public bool SprintToggle => Input.GetKey(KeyCode.LeftShift);
    public bool Jump => Input.GetKeyDown(KeyCode.Space);
    public bool Interact => Input.GetKeyDown(KeyCode.E);
    public bool OpenInventory => Input.GetKeyDown(KeyCode.I);
    public bool OpenQuest => Input.GetKeyDown(KeyCode.Q);
}
