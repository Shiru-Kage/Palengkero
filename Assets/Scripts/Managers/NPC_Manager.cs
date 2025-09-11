using UnityEngine;
using TMPro;

public class NPC_Manager : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private TextMeshProUGUI currentNPCcount;
    private int npcsToSpawnAtOnce;
    private float spawnInterval;

    [Header("Timer Settings")]
    [SerializeField] private Timer timer;

    [Header("Spawn Point Settings")]
    [SerializeField] private Transform spawnPoint;

    private float timeElapsed = 0f;
    private GameObject[] npcTypes;
    private int[] npcTypeLikelihoods;

    private void Start()
    {
        LevelData currentLevelData = LevelStateManager.Instance.GetCurrentLevelData();

        if (currentLevelData != null)
        {
            npcsToSpawnAtOnce = Random.Range(currentLevelData.minNPCToSpawn, currentLevelData.maxNPCToSpawn + 1);
            spawnInterval = Random.Range(currentLevelData.minSpawnInterval, currentLevelData.maxSpawnInterval);

            npcTypes = currentLevelData.typeOfNPCs;
            npcTypeLikelihoods = currentLevelData.npcTypeLikelihoods;

            if (npcTypes.Length != npcTypeLikelihoods.Length)
            {
                Debug.LogWarning("typeOfNPCs and npcTypeLikelihoods length mismatch! Defaulting to equal likelihoods.");
                npcTypeLikelihoods = new int[npcTypes.Length];
                for (int i = 0; i < npcTypes.Length; i++)
                    npcTypeLikelihoods[i] = 1;
            }
        }
        else
        {
            Debug.LogError("LevelData is missing. NPC spawn settings will not be applied.");
        }
    }

    private void Update()
    {
        if (TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.IsTutorialActive()) return;
        }
        if (timer.IsRunning)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= spawnInterval)
            {
                SpawnNPCs();
                timeElapsed = 0f;
            }
        }
        UpdateNPCCountUI();
    }

    private void SpawnNPCs()
    {
        int currentNPCs = GameObject.FindObjectsByType<NPC_Shopper>(FindObjectsSortMode.None).Length;
        LevelData currentLevelData = LevelStateManager.Instance.GetCurrentLevelData();
        int maxNPCInLevel = currentLevelData.NPCperLevel;

        int spawnableNPCs = Mathf.Min(npcsToSpawnAtOnce, maxNPCInLevel - currentNPCs);
        if (spawnableNPCs <= 0) return;

        for (int i = 0; i < spawnableNPCs; i++)
        {
            GameObject npcToSpawn = ChooseNPCType();
            if (npcToSpawn != null)
                Instantiate(npcToSpawn, spawnPoint.position, Quaternion.identity);
        }
    }

    private GameObject ChooseNPCType()
    {
        if (npcTypes == null || npcTypes.Length == 0) return null;

        int totalWeight = 0;
        foreach (var w in npcTypeLikelihoods)
            totalWeight += w;

        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;

        for (int i = 0; i < npcTypes.Length; i++)
        {
            cumulative += npcTypeLikelihoods[i];
            if (roll < cumulative)
                return npcTypes[i];
        }

        return npcTypes[0];
    }

    private void UpdateNPCCountUI()
    {
        int currentNPCs = GameObject.FindObjectsByType<NPC_Shopper>(FindObjectsSortMode.None).Length;

        if (currentNPCcount != null)
        {
            currentNPCcount.text = $"{currentNPCs}/10";
        }
    }
}
