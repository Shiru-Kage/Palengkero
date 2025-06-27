using UnityEngine;

public class NPC_Manager : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private int npcsToSpawnAtOnce = 5; 
    [SerializeField] private float spawnInterval = 4f; 

    [Header("Timer Settings")]
    [SerializeField] private Timer timer;

    [Header("Spawn Point Settings")]
    [SerializeField] private Transform spawnPoint; 

    private float timeElapsed = 0f; 
    private int spawnCount = 0; 

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
