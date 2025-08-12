using UnityEngine;

public class NPC_Manager : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private GameObject npcPrefab;  // NPC prefab to spawn
    private int npcsToSpawnAtOnce;
    private float spawnInterval;

    [Header("Timer Settings")]
    [SerializeField] private Timer timer;

    [Header("Spawn Point Settings")]
    [SerializeField] private Transform spawnPoint; 

    private float timeElapsed = 0f; 
    private int spawnCount = 0;

    private void Start()
    {
        LevelData currentLevelData = LevelStateManager.Instance.GetCurrentLevelData();

        if (currentLevelData != null)
        {
            npcsToSpawnAtOnce = Random.Range(currentLevelData.minNPCToSpawn, currentLevelData.maxNPCToSpawn + 1);
            spawnInterval = Random.Range(currentLevelData.minSpawnInterval, currentLevelData.maxSpawnInterval);
        }
        else
        {
            Debug.LogError("LevelData is missing. NPC spawn settings will not be applied.");
        }
    }

    private void Update()
    {
        if (timer.IsRunning)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= spawnInterval)
            {
                SpawnNPCs();
                timeElapsed = 0f;
            }
        }
    }

    private void SpawnNPCs()
    {
        for (int i = 0; i < npcsToSpawnAtOnce; i++)
        {
            Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
        }

        spawnCount++;
    }
}
