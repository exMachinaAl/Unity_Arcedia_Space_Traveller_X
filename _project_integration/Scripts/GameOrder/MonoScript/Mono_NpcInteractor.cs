using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NpcState {
    Passive,
    Aggresive
}

public class Mono_NpcInteractor : MonoBehaviour
{
    // Start is called before the first frame update
    public string npcId;
    public string npcName;
    public NpcState state = NpcState.Passive;

    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }

    public void Interact()
    {
        Logger.LogNormal("NPC", $"nama: {npcName}, npc berbicara 'λο χιαο νε κοδμ ισ νφφξιερι μμμεκα'");
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {

    //     }
    // }
}
