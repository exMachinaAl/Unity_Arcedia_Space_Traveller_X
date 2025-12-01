using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Game_ResourceNode : MonoBehaviour
{
    [Header("Identification")]
    public SO_ObjekGameProperties nodeTemplateProp;
    [SerializeField]private SO_ObjekGameProperties nodeProperties;
    // public long resourceId; // set via inspector or auto-generate in editor
    // public string itemDropId = "Orion_tree"; // id item yang masuk inventory
    // public int dropAmount = 100; // banyaknya yang didapat sekali ekstrak

    [Header("Mining")]
    // public ToolType requiredTool = ToolType.Axe;
    // public float miningDuration = 50f; // detik harus hold
	
	public static MiningUI MiningUIInstance;

    // internal
    bool isBeingMined = false;
    Coroutine miningCoroutine;
    Transform playerLookTransform; // referensi dari interactor agar bisa validasi
    //planetInvatedData worldLoader;
    int planetSeed;
    int resourceIndex;

    public void Init(long id)
    {
        nodeProperties = Instantiate(nodeTemplateProp);

        //tinggal migrasi dari statik data to screiptableObjek data

        nodeProperties.id = id;
        // resourceId = id;
    }
    // void Start()
    // {
        // safety: generate id kalau kosong (untuk prototyping)
        // if (string.IsNullOrEmpty(resourceId))
        //     resourceId = $"{gameObject.name}_{Guid.NewGuid().ToString().Substring(0,8)}";


        // jika node sudah didepleted oleh save, matikan/destroy
        // if (Game_SaveSystem.Instance != null && Game_SaveSystem.Instance.IsNodeDepleted(kontol, resourceId))
        // {
        //     Destroy(gameObject); // tidak spawn karena sudah diambil sebelumnya
        // }
    // }

    public void TryStartMining(Transform playerLook, ToolType currentTool, Game_PlayerInventory inventory)
    {
        if (isBeingMined) return;
        // if (currentTool != requiredTool)
        // {
        //     // tool salah — bisa play sound atau UI feedback
        //     Debug.Log("Wrong tool. Need: " + requiredTool);
        //     return;
        // }
        ItemStack tool = inventory.GetMainHandItem();

        float harvestTime = nodeProperties.durabilty;
        int reward = nodeProperties.dropAmount;

        // =========================
        // VALIDASI TOOL
        // =========================
        if (tool != null && tool.item.itemType == ItemType.Tool)
        {
            if (tool.item.toolType == nodeProperties.requiredTool)
            {
                // ✅ TOOL SESUAI
                harvestTime /= tool.item.efficiency; // makin cepat
            }
            else
            {
                // ⚠️ TOOL SALAH
                harvestTime *= 2f;
                reward = Mathf.Max(1, reward / 2);
            }

            tool.currentDurability--;
        }
        else
        {
            // ❌ TANGAN KOSONG
            harvestTime *= 3f;
            reward = 1;
        }

        // miningCoroutine = StartCoroutine(MineRoutine(playerLook, playerInventory));
        miningCoroutine = StartCoroutine(MineRoutineV2(playerLook, inventory, harvestTime, reward));
    }

    public void CancelMining()
    {
        if (isBeingMined && miningCoroutine != null)
        {
            StopCoroutine(miningCoroutine);
            miningCoroutine = null;
            isBeingMined = false;
            // feedback
            Debug.Log("Mining cancelled");
        }
    }

    IEnumerator MineRoutineV2(Transform playerLook, Game_PlayerInventory inventory, float harvestTime, int reward)
    {
		isBeingMined = true;
		float elapsed = 0f;

		// Show UI
		if (MiningUIInstance != null) MiningUIInstance.Show();

        while (elapsed < harvestTime)
		{
			Vector3 toNode = (transform.position - playerLook.position).normalized;
			float dot = Vector3.Dot(playerLook.forward, toNode);

			// Kalau crosshair pindah / player lepas tombol
			if (dot < 0.95f)
			{
				CancelMiningUI();
				isBeingMined = false;
				yield break;
			}

			// elapsed += 0.7f * Time.deltaTime;
			elapsed += Time.deltaTime;

			// Update UI
			if (MiningUIInstance != null)
				MiningUIInstance.SetProgress(elapsed / harvestTime);

			yield return null;
		}

		CancelMiningUI();
		OnMinedV2(inventory, reward);
		isBeingMined = false;

        // =========================
        // PROSES DAMAGE (SIMULASI)
        // =========================
        // currentHP -= 25;

        // if (currentHP <= 0)
        // {
        //     inventory.AddItem(dropItem, reward);
        //     Destroy(gameObject);
        // }
    }
    // IEnumerator MineRoutine(Transform playerLook, Game_PlayerInventory playerInventory)
	// {
	// 	isBeingMined = true;
	// 	float elapsed = 0f;

	// 	// Show UI
	// 	if (MiningUIInstance != null) MiningUIInstance.Show();

	// 	while (elapsed < miningDuration)
	// 	{
	// 		Vector3 toNode = (transform.position - playerLook.position).normalized;
	// 		float dot = Vector3.Dot(playerLook.forward, toNode);

	// 		// Kalau crosshair pindah / player lepas tombol
	// 		if (dot < 0.95f)
	// 		{
	// 			CancelMiningUI();
	// 			isBeingMined = false;
	// 			yield break;
	// 		}

	// 		// elapsed += 0.7f * Time.deltaTime;
	// 		elapsed += Time.deltaTime;

	// 		// Update UI
	// 		if (MiningUIInstance != null)
	// 			MiningUIInstance.SetProgress(elapsed / miningDuration);

	// 		yield return null;
	// 	}

	// 	CancelMiningUI();
	// 	OnMined(playerInventory);
	// 	isBeingMined = false;
	// }

	void CancelMiningUI()
	{
		if (MiningUIInstance != null)
			MiningUIInstance.Hide();
	}


    void OnMinedV2(Game_PlayerInventory playerInventory, int reward)
    {
        Debug.Log($"Node {nodeProperties.id} mined. Give {reward}x {nodeProperties.dropItem.id}");

        // add to inventory
        playerInventory.AddItem(nodeProperties.dropItem, reward);
		
		// ada quest
		// Manager_Quest.Instance.CollectItem(itemDropId, dropAmount); // prbaikan menjadi update statik berdasarkan inventory

        // mark in save
        if (Game_SaveSystem.Instance != null)
            Game_SaveSystem.Instance.MarkNodeDepleted(Game_SaveSystem.Instance.GetCurrentPlanetId(), nodeProperties.id);

        Destroy(gameObject);
    }
    // void OnMined(Game_PlayerInventory playerInventory)
    // {
    //     // Debug.Log($"Node {resourceId} mined. Give {dropAmount}x {itemDropId}");
    //     Debug.Log($"Node {nodeProperties.id} mined. Give {nodeProperties.dropAmount}x {nodeProperties.dropItem.id}");

    //     // add to inventory
    //     // playerInventory.AddItem(itemDropId, dropAmount);
    //     playerInventory.AddItem(nodeProperties.dropItem, nodeProperties.dropAmount);
		
	// 	// ada quest
	// 	// QuestManager.Instance.CollectItem(itemDropId, dropAmount);
	// 	// Manager_Quest.Instance.CollectItem(itemDropId, dropAmount); // prbaikan menjadi update statik berdasarkan inventory

    //     // mark in save
    //     if (Game_SaveSystem.Instance != null)
    //         Game_SaveSystem.Instance.MarkNodeDepleted(Game_SaveSystem.Instance.GetCurrentPlanetId(), nodeProperties.id);

    //     // visual feedback: spawn VFX, sound, etc.
    //     // lalu destroy object
    //     Destroy(gameObject);
    // }
}
