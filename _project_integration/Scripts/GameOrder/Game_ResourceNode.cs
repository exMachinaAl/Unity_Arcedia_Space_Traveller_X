using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Game_ResourceNode : MonoBehaviour
{
    [Header("Identification")]
    public long resourceId; // set via inspector or auto-generate in editor
    public string itemDropId = "Orion_tree"; // id item yang masuk inventory
    public int dropAmount = 100; // banyaknya yang didapat sekali ekstrak

    [Header("Mining")]
    public ToolType requiredTool = ToolType.Axe;
    public float miningDuration = 3f; // detik harus hold
	
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
        resourceId = id;
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

    public void TryStartMining(Transform playerLook, ToolType currentTool, Game_PlayerInventory playerInventory)
    {
        if (isBeingMined) return;
        if (currentTool != requiredTool)
        {
            // tool salah — bisa play sound atau UI feedback
            Debug.Log("Wrong tool. Need: " + requiredTool);
            return;
        }

        miningCoroutine = StartCoroutine(MineRoutine(playerLook, playerInventory));
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

    IEnumerator MineRoutine(Transform playerLook, Game_PlayerInventory playerInventory)
	{
		isBeingMined = true;
		float elapsed = 0f;

		// Show UI
		if (MiningUIInstance != null) MiningUIInstance.Show();

		while (elapsed < miningDuration)
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

			elapsed += Time.deltaTime;

			// Update UI
			if (MiningUIInstance != null)
				MiningUIInstance.SetProgress(elapsed / miningDuration);

			yield return null;
		}

		CancelMiningUI();
		OnMined(playerInventory);
		isBeingMined = false;
	}

	void CancelMiningUI()
	{
		if (MiningUIInstance != null)
			MiningUIInstance.Hide();
	}


    void OnMined(Game_PlayerInventory playerInventory)
    {
        Debug.Log($"Node {resourceId} mined. Give {dropAmount}x {itemDropId}");

        // add to inventory
        playerInventory.AddItem(itemDropId, dropAmount);
		
		// ada quest
		QuestManager.Instance.CollectItem(itemDropId, dropAmount);

        // mark in save
        if (Game_SaveSystem.Instance != null)
            Game_SaveSystem.Instance.MarkNodeDepleted(Game_SaveSystem.Instance.getCurrentPlanetId(), resourceId);

        // visual feedback: spawn VFX, sound, etc.
        // lalu destroy object
        Destroy(gameObject);
    }
}
