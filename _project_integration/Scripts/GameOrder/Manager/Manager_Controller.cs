// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum GameControlMode
{
    Gameplay,
    UIMode
}


public class Manager_Controller : MonoBehaviour
{
    public static Manager_Controller Instance;

    public GameControlMode mode = GameControlMode.Gameplay;
    public MobileInput mobileInput;
    public PCInput pcInput;
    public IPlayerInput CInput;

    void Awake()
    {
        if (Manager_Controller.Instance != null && Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // SetGameplayMode(); // default
        SetInputPlayer();
    }

    public void SetInputPlayer()
    {
        mobileInput = gameObject.AddComponent<MobileInput>();
        pcInput = gameObject.AddComponent<PCInput>();

        // #if UNITY_ANDROID || UNITY_IOS
        //     CInput = mobileInput;
        //     Debug.Log("[Controller] using Mobile controll");
        //     // Manager_Player.Instance.humanCtrl.input = mobileInput;
        // #else
        //     // Manager_Player.Instance.humanCtrl.input = pcInput;
        //     CInput = pcInput;
        // Debug.Log("[Controller] using PC controll");
        // #endif

        CInput = pcInput;
    }
    public void SetGameplayMode()
    {
        mode = GameControlMode.Gameplay;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // aktifkan player controller
        switch (Game_SaveSystem.Instance.GetPlayerMode())
        {
            case PlayerMode.Human:
                {
                    Manager_Player.Instance.humanCtrl?.EnableControl();
                    break;
                }
            case PlayerMode.Flight:
                {
                    Manager_Player.Instance.flightCtrl?.EnableGameControl();
                    break;
                }
            default:
                {
                    Debug.LogWarning($"switch error for Manager controller");
                    break;
                }
        }
    }

    public void SetUIMode()
    {
        mode = GameControlMode.UIMode;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // matikan player controller
        switch (Game_SaveSystem.Instance.GetPlayerMode())
        {
            case PlayerMode.Human:
                {
                    Manager_Player.Instance.humanCtrl?.DisableControl();
                    break;
            }
            case PlayerMode.Flight:
                {
                    Manager_Player.Instance.flightCtrl?.DisableGameControl();
                    break;
            }
            default: {
                    Debug.LogWarning($"switch error for Manager controller");
                    break;
            }
        }
    }
}
