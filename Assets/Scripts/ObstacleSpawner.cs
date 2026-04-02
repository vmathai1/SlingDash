using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject groundPoopPrefab;
    public GameObject hangingPoopPrefab;

    [Header("Spawn Settings")]
    public float minSpawnInterval = 1.5f;
    public float maxSpawnInterval = 3.5f;
    public float spawnX = 10f;
    public float groundY = 0f;
    public float hangingY = 4f;
    public float hangingPoopChance = 0.3f;

    [Header("Difficulty Scaling")]
    public float minPossibleInterval = 0.4f;
    public float maxSpeedMultiplier = 3f;
    public int scorePerDifficultyLevel = 5;

    private float timer;
    private float nextSpawn;
    private int lastScore = 0;

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
        int currentScore = GameManager.Instance.GetScore();

        float difficultyLevel = currentScore / scorePerDifficultyLevel;

        float intervalReduction = difficultyLevel * 0.15f;
        float currentMin = Mathf.Max(minPossibleInterval,
                           minSpawnInterval - intervalReduction);
        float currentMax = Mathf.Max(minPossibleInterval + 0.3f,
                           maxSpawnInterval - intervalReduction);

        nextSpawn = Random.Range(currentMin, currentMax);

        hangingPoopChance = Mathf.Min(0.6f,
                            0.3f + (difficultyLevel * 0.05f));

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
        bool spawnHanging = Random.value < hangingPoopChance;

        if (spawnHanging)
        {
            Vector3 pos = new Vector3(spawnX, hangingY, 0);
            GameObject poop = Instantiate(
                hangingPoopPrefab, pos, Quaternion.identity);
            poop.tag = "HangingPoop";
            poop.AddComponent<ObstacleMover>();
        }
        else
        {
            Vector3 pos = new Vector3(spawnX, groundY, 0);
            GameObject poop = Instantiate(
                groundPoopPrefab, pos, Quaternion.identity);
            poop.tag = "GroundPoop";
            poop.AddComponent<ObstacleMover>();
        }
    }
}

