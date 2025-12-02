using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_MenuInteractNpc : MonoBehaviour
{
    public GameObject interactionMenu;
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    private List<GameObject> buttons = new List<GameObject>();


    public void SetShowMenuInteractNpc(bool show)
    {
        interactionMenu.SetActive(show);
    }
    // Memperbarui UI dengan daftar NPC yang bisa berinteraksi
    public void UpdateInteractableNPCs(List<Mono_NpcInteractor> npcList)
    {
        // Hapus tombol yang lama
        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();

        // Buat tombol baru untuk setiap NPC
        foreach (Mono_NpcInteractor npc in npcList)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            newButton.GetComponentInChildren<TMP_Text>().text = npc.npcName;
            newButton.GetComponent<Button>().onClick.AddListener(() => InteractWithNPC(npc));
            buttons.Add(newButton);
        }

        // Atur scrollable UI jika ada lebih dari 4 NPC
        interactionMenu.SetActive(npcList.Count > 0);
    }

    // Aksi interaksi dengan NPC yang dipilih
    private void InteractWithNPC(Mono_NpcInteractor npc)
    {
        npc.Interact();
    }
}
