using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject groundPoopPrefab;
    public GameObject hangingPoopPrefab;
    public GameObject springPrefab;
    public GameObject hangingTopPrefab;
    public GameObject fallingTopPrefab;

    [Header("Spawn Settings")]
    public float minSpawnInterval = 1.5f;
    public float maxSpawnInterval = 3.5f;
    public float spawnX = 10f;
    public float groundY = -3.1f;
    public float hangingY = -3.5f;
    public float hangingTopY = 4f;
    public float hangingPoopChance = 0.2f;
    public float springChance = 0.2f;
    public float hangingTopChance = 0.1f;
    public float fallingTopChance = 0.1f;

    [Header("Difficulty Scaling")]
    public float minPossibleInterval = 1.2f;
    public float maxSpeedMultiplier = 3f;
    public int scorePerDifficultyLevel = 5;

    float timer;
    float nextSpawn;
    int lastScore = 0;

    void Start() => ScheduleNextSpawn();

    void Update()
    {
        if (GameManager.Instance.IsGameOver()) return;

        timer += Time.deltaTime;

        if (timer >= nextSpawn)
        {
            SpawnObstacle();
            timer = 0f;
            ScheduleNextSpawn();
        }
    }

    void ScheduleNextSpawn()
    {
        int currentScore = Mathf.FloorToInt(GameManager.Instance.GetScore());
        float difficultyLevel = currentScore / scorePerDifficultyLevel;

        float intervalReduction = difficultyLevel * 0.15f;
        float currentMin = Mathf.Max(minPossibleInterval,
                           minSpawnInterval - intervalReduction);
        float currentMax = Mathf.Max(minPossibleInterval + 0.3f,
                           maxSpawnInterval - intervalReduction);

        nextSpawn = Random.Range(currentMin, currentMax);
        hangingPoopChance = Mathf.Min(0.3f,
                            0.2f + (difficultyLevel * 0.01f));

        if (currentScore != lastScore)
        {
            Debug.Log($"Score: {currentScore} | " +
                     $"Interval: {currentMin:F1}-{currentMax:F1}s");
            lastScore = currentScore;
        }
    }

    void SpawnObstacle()
    {
        float roll = Random.value;

        Debug.Log($"Spawning roll: {roll:F2}");

        // Spring on ground
        if (roll < springChance && springPrefab != null)
        {
            Debug.Log("Spawning Spring");
            Vector3 pos = new Vector3(spawnX, groundY, 0);
            GameObject spring = Instantiate(
                springPrefab, pos, Quaternion.identity);
            spring.tag = "Spring";
            spring.AddComponent<ObstacleMover>();
        }
        // Hanging top — stays
        else if (roll < springChance + hangingTopChance
                 && hangingTopPrefab != null)
        {
            Debug.Log("Spawning HangingTop");
            Vector3 pos = new Vector3(spawnX, hangingTopY, 0);
            GameObject obs = Instantiate(
                hangingTopPrefab, pos, Quaternion.identity);
            obs.tag = "DeadlyObstacle";
            obs.AddComponent<ObstacleMover>();
        }
        // Falling top
        else if (roll < springChance + hangingTopChance + fallingTopChance
                 && fallingTopPrefab != null)
        {
            Debug.Log("Spawning FallingTop");
            // Spawn closer so player has time to react
            Vector3 pos = new Vector3(spawnX * 0.6f, hangingTopY, 0);
            GameObject obs = Instantiate(
                fallingTopPrefab, pos, Quaternion.identity);
            obs.tag = "DeadlyObstacle";
            // No ObstacleMover — FallingObstacle handles its own movement
        }
        // Deadly hanging from middle
        else if (roll < springChance + hangingTopChance + fallingTopChance
                 + hangingPoopChance && hangingPoopPrefab != null)
        {
            Debug.Log("Spawning HangingPoop");
            Vector3 pos = new Vector3(spawnX, hangingY, 0);
            GameObject obs = Instantiate(
                hangingPoopPrefab, pos, Quaternion.identity);
            obs.tag = "DeadlyObstacle";
            obs.AddComponent<ObstacleMover>();
        }
        // Ground obstacle — most common
        else if (groundPoopPrefab != null)
        {
            Debug.Log("Spawning Ground obstacle");
            Vector3 pos = new Vector3(spawnX, groundY, 0);
            GameObject obs = Instantiate(
                groundPoopPrefab, pos, Quaternion.identity);
            obs.tag = "Obstacle";
            obs.AddComponent<ObstacleMover>();
        }
        else
        {
            Debug.LogWarning("Nothing spawned! Check prefab assignments!");
        }
    }
}