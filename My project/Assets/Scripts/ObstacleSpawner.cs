using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject groundPoopPrefab;
    public GameObject hangingPoopPrefab;
    public GameObject springPrefab;

    [Header("Spawn Settings")]
    public float minSpawnInterval = 1.5f;
    public float maxSpawnInterval = 3.5f;
    public float spawnX = 10f;
    public float groundY = -3.1f;
    public float hangingY = -3.5f;
    public float hangingPoopChance = 0.3f;
    public float springChance = 0.2f;

    [Header("Difficulty Scaling")]
    public float minPossibleInterval = 0.8f;
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
        hangingPoopChance = Mathf.Min(0.6f, 0.3f + (difficultyLevel * 0.05f));

        if (currentScore != lastScore)
        {
            Debug.Log($"Score: {currentScore} | " +
                     $"Interval: {currentMin:F1}-{currentMax:F1}s | " +
                     $"Hanging: {hangingPoopChance:F2}");
            lastScore = currentScore;
        }
    }

    void SpawnObstacle()
    {
        float roll = Random.value;

        // Spring spawns on ground
        if (roll < springChance && springPrefab != null)
        {
            Vector3 pos = new Vector3(spawnX, groundY, 0);
            GameObject spring = Instantiate(springPrefab, pos, Quaternion.identity);
            spring.tag = "Spring";
            spring.AddComponent<ObstacleMover>();
        }
        // Deadly hanging obstacle
        else if (roll < springChance + hangingPoopChance && hangingPoopPrefab != null)
        {
            Vector3 pos = new Vector3(spawnX, hangingY, 0);
            GameObject obs = Instantiate(hangingPoopPrefab, pos, Quaternion.identity);
            obs.tag = "DeadlyObstacle";
            obs.AddComponent<ObstacleMover>();
        }
        // Ground obstacle
        else if (groundPoopPrefab != null)
        {
            Vector3 pos = new Vector3(spawnX, groundY, 0);
            GameObject obs = Instantiate(groundPoopPrefab, pos, Quaternion.identity);
            obs.tag = "Obstacle";
            obs.AddComponent<ObstacleMover>();
        }
    }
}