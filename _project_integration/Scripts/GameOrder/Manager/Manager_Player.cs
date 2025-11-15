using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerMode { Human, Flight }
public enum PlayerInThe { Ground, Space }

public class Manager_Player : MonoBehaviour
{
    public static Manager_Player Instance;

    public GameObject player;             // Player object
    public PlayerControllerV4 humanCtrl;    // Human movement
    public FlightControllerV1 flightCtrl;   // Flight movement
    public Camera playerCam;
    public Camera shipCam;
    public GameObject cameraPivot;
    public PlayerMode mode = PlayerMode.Human;
    public PlayerInThe InWorld = PlayerInThe.Ground;


    // public float interactRange = 6f;
    // public LayerMask interactLayerMask;

    void Awake()
    {
        // Prevent duplicate managers
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // void Update()
    // {
    //     if (mode == PlayerMode.Human && player != null && playerCam != null) // akses interaksi
    //     OnTriggerStay();    
    // }

    public void RegisterPlayer(GameObject p)
    {
        player = p;
        humanCtrl = p.GetComponent<PlayerControllerV4>();
        playerCam = p.GetComponentInChildren<Camera>();
        cameraPivot = playerCam.transform.parent.gameObject;
    }

    // public void SwitchToHuman()
    // {
    //     player.GetComponent<PlayerControllerV4>().enabled = true;
    //     flightCtrl.enabled = false;
    //     cameraPivot.gameObject.SetActive(true);
    //     shipCam.gameObject.SetActive(false);
    // }

    // public void SwitchToFlight(FlightControllerV1 ship)
    // {
    //     player.GetComponent<PlayerControllerV4>().enabled = false;
    //     flightCtrl = ship.GetComponent<FlightControllerV1>();
    //     flightCtrl.enabled = true;

    //     cameraPivot.gameObject.SetActive(false);
    //     shipCam.gameObject.SetActive(true);
    // }


    public void EnterShip(FlightControllerV1 ship)
    {
        mode = PlayerMode.Flight;

        // Hide player model
        player.SetActive(false);

        // Enable ship controller
        ship.EnableControl();
        flightCtrl = ship.GetComponent<FlightControllerV1>();

        // Switch camera
        playerCam.gameObject.SetActive(false);
        shipCam = ship.shipCam;
        shipCam.gameObject.SetActive(true);

        // Make ship DDOL
        DontDestroyOnLoad(ship.gameObject);
    }

    public void ExitShip(FlightControllerV1 ship, Transform exitPoint, Scene planetScene)
    {
        mode = PlayerMode.Human;

        // Disable ship control
        ship.DisableControl();

        // Move ship back to planet scene
        SceneManager.MoveGameObjectToScene(ship.gameObject, planetScene);

        // Show player
        player.transform.position = exitPoint.position;
        player.SetActive(true);

        // Switch cameras
        shipCam.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);
    }

    // private void OnTriggerStay()
    // {
    //     Ray ray = new Ray(cam.position, cam.forward);

    //     if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayerMask))
    //     {
    //         FlightControllerV1 ship = hit.collider.GetComponentInParent<FlightControllerV1>();

    //         Debug.Log("hit something smotththh");
    //         if (ship != null)
    //         {
    //             Debug.Log("Press F to enter the ship");
    //             if (Input.GetKeyDown(KeyCode.F))
    //             {
    //                 // Interact with ship
    //                 Manager_Player.Instance.EnterShip(ship);
    //             }
    //         }
    //     }
    // }
}
