using UnityEngine;

public class Game_PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public float interactRange = 4f;
    public LayerMask interactLayerMask;
    public Game_PlayerInventory inventory;
    public Game_ToolController toolController;

    Game_ResourceNode currentNode;
    bool isHoldingInteract = false;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Manager_Player.Instance.playerCam;
        }
    }

    void Update()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayerMask)) // coba tambahin interak ke intreak abel objek
        // nih disini untuk pesawat deh nanti
        {
            Debug.Log($"Hit somewting: {hit.collider.name}");
            Game_ResourceNode node = hit.collider.GetComponentInParent<Game_ResourceNode>();
            if (node != null)
            {
                // show UI hint: "Hold F to mine" etc (implement your UI)
                // jika player mulai menekan tombol interact, mulai mining
                //if (Input.GetButtonDown("Fire1")) // atau tombol lain, misal "E" / "Use"
				if (Input.GetKeyDown(KeyCode.E)) // atau tombol lain, misal "E" / "Use"
                {
                    currentNode = node;
                    isHoldingInteract = true;
                    currentNode.TryStartMining(playerCamera.transform, toolController.currentTool, inventory);
                }
                //else if (Input.GetButtonUp("Fire1"))
				else if (Input.GetKeyUp(KeyCode.E))
                {
                    isHoldingInteract = false;
                    if (currentNode != null) currentNode.CancelMining();
                    currentNode = null;
                }
            }
            else
            {
                // tidak mengarah ke resource node
                if (isHoldingInteract && currentNode != null)
                {
                    currentNode.CancelMining();
                    isHoldingInteract = false;
                    currentNode = null;
                }
            }
        }
        else
        {
            // tidak ada raycast hit
            if (isHoldingInteract && currentNode != null)
            {
                currentNode.CancelMining();
                isHoldingInteract = false;
                currentNode = null;
            }
        }
    }
}
