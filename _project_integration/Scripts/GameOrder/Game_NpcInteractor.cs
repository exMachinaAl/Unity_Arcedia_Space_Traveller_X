using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_NpcInteractor : MonoBehaviour
{
    public string interactText = "F";
    public bool playerInRange = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Game_InteractionUI.Instance.ShowText(interactText, transform);
            //Game_OutlineEffect.Instance.HighlightObject(GetComponent<Renderer>());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Game_InteractionUI.Instance.HideText();
            //Game_OutlineEffect.Instance.RemoveHighlight();
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Player interacted with: " + name);
        }
    }
}
