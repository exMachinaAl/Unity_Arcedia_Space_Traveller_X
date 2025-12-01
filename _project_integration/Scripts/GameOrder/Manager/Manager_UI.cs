using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class InventorySlotUI
{
    public Image iconImage;
    public TMP_Text amountText;
    public Image uiSelected;

    public void UpdateSlot(ItemStack itemStack)
    {
        if (itemStack != null && itemStack.amount > 0)
        {
            iconImage.sprite = itemStack.item.icon;
            iconImage.enabled = true;
            amountText.text = itemStack.amount.ToString();
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            amountText.text = "";
        }
    }
}
public class Manager_UI : MonoBehaviour
{

    [Header("Main Manager UI")]
    public static Manager_UI Instance;
    public UI_MenuInteractNpc UIMenuInteract;


    [Header("Manager UI | Inventory")]
    public GameObject rootQuickInventory;
    public GameObject slotInventoryPrefab;
    public int maxShowSlot = 9;
    public InventorySlotUI[] inventorySlots;

    //ini button semua
    public Button btn_questMenu;
    public Button btnClose_questMenu;

    //ini canvas gorup per ui
    public CanvasGroup grp_QuestUI;
    // public RectTransform questMini;
    public GameObject fullQuestMenu;

    public GameObject miniPanelQuest;
    public TMP_Text currentMiniQuest_Title;
    public TMP_Text currentMiniQuest_objective;
    // public TMP_Text titleText;
    // public TMP_Text descriptionText;
    public TMP_Text fullListQuest;

    //yapping panel
    public GameObject yappingPanel;
    public bool isYappingPanelOn = false;

    void Awake()
    {
        // Prevent duplicate managers
        if (Instance != null && Instance != this && Manager_UI.Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        btn_questMenu.onClick.AddListener(OnShowQuestMenu);
        btnClose_questMenu.onClick.AddListener(OnCloseQuestMenu);

        miniPanelQuest.SetActive(false);
        btn_questMenu.gameObject.SetActive(false);
        HideYapping();

        //# inventory start sect
        InitSlotInventoryUI();
    }

    void Update()
    {
        if (isYappingPanelOn && Input.GetKeyDown(KeyCode.M))
            HideYapping();

        if (Input.GetKeyDown(KeyCode.Q))
            OnShowQuestMenu();

        // ui untuk gonta ganti main hand inventory
        // InitSlotInventoryUI();
    }

    //# UI Inventory Here
    public void InitSlotInventoryUI()
    {
        // Image[] slotBorders = new Image[maxShowSlot];
        inventorySlots = new InventorySlotUI[maxShowSlot];

        for (int i = 0; i < maxShowSlot; i++)
        {
            GameObject slotObj = Instantiate(slotInventoryPrefab, rootQuickInventory.transform);
            var icon = slotObj.transform.Find("Icon").GetComponent<Image>();
            var amountText = slotObj.transform.Find("Amount").GetComponent<TextMeshProUGUI>();

            inventorySlots[i] = new InventorySlotUI
            {
                iconImage = icon,
                amountText = amountText,
                uiSelected = slotObj.GetComponent<Image>()
            };

            // if (inventorySlots[i] != null)
            // {
            //     icon.sprite = inventorySlots[i].iconImage.sprite;
            //     amountText.text = inventorySlots[i].amountText.text;
            // }
            // else
            // {
            //     icon.enabled = false;
            //     amountText.text = "";
            // }


            // slotBorders[i] = slotObj.GetComponent<Image>();
        }

        // HighlightSlot();
    }
    public void HighlightSlot(int uiIndexForSelected)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].uiSelected.color = (i == uiIndexForSelected) ? Color.yellow : Color.white;
        }
    }
    public void OnChangeOrUpdateInventory(Game_PlayerInventory inventory, int selectedIndex)
    {
        // Pastikan selectedIndex tidak lebih kecil dari 0 atau lebih besar dari inventory item count
        selectedIndex = Mathf.Clamp(selectedIndex, 0, inventory.GetItemCount() - 1);

        // Hitung batas kiri dan kanan berdasarkan selectedIndex
        int halfSlot = maxShowSlot / 2;  // Karena ada 9 slot, halfSlot akan menjadi 4
        int minIndex = Mathf.Max(0, selectedIndex - halfSlot); // Pastikan tidak lebih kecil dari 0
        int maxIndex = Mathf.Min(inventory.GetItemCount() - maxShowSlot, selectedIndex - halfSlot);

        // Jika selectedIndex terlalu dekat dengan kiri, geser ke kanan
        if (selectedIndex <= halfSlot)
        {
            minIndex = 0;
        }
        // Jika selectedIndex terlalu dekat dengan kanan, geser ke kiri
        else if (selectedIndex >= inventory.GetItemCount() - halfSlot)
        {
            minIndex = inventory.GetItemCount() - maxShowSlot;
        }

        // Tentukan slot UI yang aktif
        int uiIndexForSelected = selectedIndex - minIndex;

        // Update posisi tampilan untuk menunjukkan item-item sesuai dengan index yang dihitung
        for (int i = 0; i < maxShowSlot; i++)
        {
            int itemIndex = minIndex + i;
            // Update UI untuk item ke-`itemIndex`
            UpdateInventorySlotUI(i, itemIndex, inventory);
        }
        HighlightSlot(uiIndexForSelected);
    }

    // Fungsi untuk memperbarui UI setiap slot (misalnya gambar item, jumlah, dll)
    void UpdateInventorySlotUI(int slotIndex, int itemIndex, Game_PlayerInventory inventory)
    {
        // Asumsi ada method untuk mendapatkan item berdasarkan index
        var item = inventory.GetItemAtIndex(itemIndex);
        // inventory.GetMainHandItem();
        // Update UI slot ke slotIndex dengan informasi item yang sesuai
        inventorySlots[slotIndex].UpdateSlot(item);
    }


    public void OnShowQuestMenu()
    {
        Manager_Controller.Instance.SetUIMode();
        // Debug.LogWarning("woi error nih logikanya");
        if (Manager_Quest.Instance == null)
            return;

        flushUiPanel();

        fullQuestMenu.SetActive(true);
        SetFullListQuest();
    }
    // public void OnCloseQuestMenu() => fullQuestMenuPanel.
    public void OnCloseQuestMenu()
    {
        Manager_Controller.Instance.SetGameplayMode();
        fullQuestMenu.SetActive(false);
        miniPanelQuest.SetActive(true);
        btn_questMenu.gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }

    public void SetCurrentQuest(string title, string desc)
    {
        miniPanelQuest.SetActive(true);
        btn_questMenu.gameObject.SetActive(true);
        currentMiniQuest_Title.text = $"{title}\n   {desc}";
    }
    public void ShowYapping(string text)
    {
        yappingPanel.SetActive(true);
        TMP_Text TText = yappingPanel.GetComponentInChildren<TMP_Text>();
        TText.text = $"{text}";
        isYappingPanelOn = true;
    }
    public void HideYapping()
    {
        yappingPanel.SetActive(false);
    }

    public void UpdateProgress(int current, int target)
    {
        currentMiniQuest_objective.text = $"{current}/{target}";
    }

    public void MarkObjectiveDone()
    {
        currentMiniQuest_objective.text = "<color=#00FFFF>✔ Objective Complete</color>";
    }

    public void ShowReturnHint(string npc)
    {
        currentMiniQuest_objective.text += $"Return to <b>{npc}</b>\n";
    }

    void SetFullListQuest()
    {
        fullListQuest.text = "";

        // foreach (var category in Manager_Quest.Instance.quests)
        {
            var category = Manager_Quest.Instance.quests;

            fullListQuest.text += $"-- Main Quest --\n";
            foreach (var mainQT in category.mainQuest)
            {
                fullListQuest.text += $"- <b>{mainQT.questTitle}</b>\n";
                fullListQuest.text += $"     {mainQT.questDescription}\n";
                // fullListQuest.text += $"<b>{mainQT.subTitleQuest}</b>";
            }
            fullListQuest.text += $"-- Side Quest --\n";
            foreach (var sideQT in category.sideQuest)
            {
                fullListQuest.text += $"- <b>{sideQT.questTitle}</b>\n";
                fullListQuest.text += $"     {sideQT.questDescription}\n";
                // fullListQuest.text += $"<b>{mainQT.subTitleQuest}</b>";
            }
        }
    }

    void flushUiPanel()
    {
        miniPanelQuest.SetActive(false);
        btn_questMenu.gameObject.SetActive(false);
        yappingPanel.SetActive(false);
    }
}
