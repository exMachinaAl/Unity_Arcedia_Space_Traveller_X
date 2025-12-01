using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickbarManager : MonoBehaviour
{
    public Transform slotParent;
    public GameObject slotPrefab;
    public ItemData[] items;
    private int selectedIndex = 0;
    private Image[] slotBorders;

    void Start()
    {
        slotBorders = new Image[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            var slotObj = Instantiate(slotPrefab, slotParent);
            var icon = slotObj.transform.Find("Icon").GetComponent<Image>();
            var amountText = slotObj.transform.Find("Amount").GetComponent<TextMeshProUGUI>();

            if (items[i] != null)
            {
                icon.sprite = items[i].icon;
                amountText.text = Random.Range(1, 64).ToString(); // auto isi jumlah
            }
            else
            {
                icon.enabled = false;
                amountText.text = "";
            }

            slotBorders[i] = slotObj.GetComponent<Image>();
        }

        HighlightSlot();
    }

    void Update()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                selectedIndex = i;
                HighlightSlot();
            }
        }
    }

    void HighlightSlot()
    {
        for (int i = 0; i < slotBorders.Length; i++)
        {
            slotBorders[i].color = (i == selectedIndex) ? Color.yellow : Color.white;
        }
    }
}
