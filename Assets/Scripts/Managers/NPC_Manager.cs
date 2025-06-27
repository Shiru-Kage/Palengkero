using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC_Manager : MonoBehaviour
{
    [Header("NPC Prefabs & Data")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private List<NPCData> npcDatas;

    [Header("Spawning Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int initialSpawnCount = 2;
    [SerializeField] private int maxNPCs = 10;
    [SerializeField] private float spawnInterval = 15f;

    [Header("Timer Reference")]
    [SerializeField] private Timer gameTimer;

    private List<NPC_Shopper> activeNPCs = new List<NPC_Shopper>();

    private void Start()
    {
        // Initial spawn
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnNPC();
        }

        if (gameTimer != null)
        {
            StartCoroutine(SpawnOverTime());
        }
        else
        {
            Debug.LogWarning("NPC_Manager: No Timer assigned. NPCs will not spawn over time.");
        }
    }

    private IEnumerator SpawnOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (activeNPCs.Count < maxNPCs)
            {
                SpawnNPC();
            }
        }
    }

    private void SpawnNPC()
    {
        if (npcPrefab == null || npcDatas.Count == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("NPC_Manager: Cannot spawn NPC — check prefab, datas, or spawn points.");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        NPCData chosenData = npcDatas[Random.Range(0, npcDatas.Count)];

        GameObject npcObj = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
        NPC_Shopper shopper = npcObj.GetComponent<NPC_Shopper>();
        if (shopper != null)
        {
            shopper.GetType().GetProperty(nameof(shopper.Data)).SetValue(shopper, chosenData);
        }

        activeNPCs.Add(shopper);

    }

    /// <summary>
    /// Called if you want to remove an NPC from tracking (e.g. on despawn)
    /// </summary>
    public void RemoveNPC(NPC_Shopper shopper)
    {
        if (activeNPCs.Contains(shopper))
        {
            activeNPCs.Remove(shopper);
        }
    }
}
