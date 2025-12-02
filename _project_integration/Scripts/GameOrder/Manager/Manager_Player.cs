using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum PlayerMode { Human, Flight }
public enum PlayerInThe { Ground, Space, Atmosphere }

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


    [SerializeField] private List<Mono_NpcInteractor> nearbyNPCs = new List<Mono_NpcInteractor>();

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

    // public AudioSource GetAudioSourceCurrNpc()
    // {

    // }
    public Transform GetCurrentModePlayerTransform()
    {
        switch (Game_SaveSystem.Instance.GetPlayerMode())
        {
            case PlayerMode.Human:
                {
                    return humanCtrl.transform;
                }
            case PlayerMode.Flight:
                {
                    return flightCtrl.transform;
                }
            default:
                {
                    Debug.LogWarning($"switch error for Manager Player get transform current player moded");
                    return null;
                }
        }
    }
    public Camera GetCurrentCameraPlayer()
    {
        switch (Game_SaveSystem.Instance.GetPlayerMode())
        {
            case PlayerMode.Human:
                {
                    return playerCam;
                }
            case PlayerMode.Flight:
                {
                    return shipCam;
                }
            default:
                {
                    Debug.LogWarning($"switch error for Manager Player get transform current player Camera moded");
                    return null;
                }
        }
    }


    public void EnterShip(FlightControllerV1 ship)
    {
        Game_SaveSystem.Instance.SetPlayerMode(PlayerMode.Flight);

        // Hide player model
        player.SetActive(false);

        // Enable ship controller
        ship.EnableControl();
        flightCtrl = ship.GetComponent<FlightControllerV1>();

        // Switch camera
        playerCam.gameObject.SetActive(false);
        shipCam = ship.shipCam;
        shipCam.gameObject.SetActive(true);

        //change player chunkWSGen
        var surface = FindObjectOfType<Game_SurfaceWorldGeneration>(true);
        // (true) = cari juga di inactive objects (Unity 2020+)
        if (surface != null)
            surface.OnChangePlayerTransform();
        else
            Debug.LogWarning("SurfaceWorldGeneration belum tersedia / scene belum load");

        // Make ship DDOL
        DontDestroyOnLoad(ship.gameObject);
    }

    public void ExitShip(FlightControllerV1 ship, Transform exitPoint, Scene planetScene)
    {
        Game_SaveSystem.Instance.SetPlayerMode(PlayerMode.Human);

        // Disable ship control
        ship.DisableControl();

        // Move ship back to planet scene
        SceneManager.MoveGameObjectToScene(ship.gameObject, planetScene);

        //change player chunkWSGen
        //change player chunkWSGen
        var surface = FindObjectOfType<Game_SurfaceWorldGeneration>(true);
        // (true) = cari juga di inactive objects (Unity 2020+)
        if (surface != null)
            surface.OnChangePlayerTransform();
        else
            Debug.LogWarning("SurfaceWorldGeneration belum tersedia / scene belum load");

        // Show player
        player.transform.position = exitPoint.position;
        player.SetActive(true);

        // Switch cameras
        shipCam.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);
    }

    public void MOnTriggerEnter(Collider other)
    {
        Mono_NpcInteractor npc = other.GetComponent<Mono_NpcInteractor>();
        if (npc != null)
        {
            nearbyNPCs.Add(npc);
            Manager_UI.Instance.UIMenuInteract.SetShowMenuInteractNpc(true);
            Manager_UI.Instance.UIMenuInteract.UpdateInteractableNPCs(nearbyNPCs);
        }
    }
    
    public void MOnTriggerExit(Collider other)
    {
        Mono_NpcInteractor npc = other.GetComponent<Mono_NpcInteractor>();
        if (npc != null)
        {
            nearbyNPCs.Remove(npc);
            Manager_UI.Instance.UIMenuInteract.SetShowMenuInteractNpc(false);
            Manager_UI.Instance.UIMenuInteract.UpdateInteractableNPCs(nearbyNPCs);
        }
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
