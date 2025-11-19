using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Manager_UI : MonoBehaviour
{
    public static Manager_UI Instance;

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
    }

    void Update()
    {
        if (isYappingPanelOn && Input.GetKeyDown(KeyCode.M))
            HideYapping();

        if (Input.GetKeyDown(KeyCode.Q))
            OnShowQuestMenu();
    }

    public void OnShowQuestMenu()
    {
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
