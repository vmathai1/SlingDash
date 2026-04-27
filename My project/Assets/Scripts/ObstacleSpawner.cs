using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject groundPoopPrefab;
    public GameObject hangingPoopPrefab;
    public GameObject springPrefab;
    public GameObject hangingTopPrefab;
    public GameObject fallingTopPrefab;

    [Header("Level 2 Prefabs")]
    public GameObject swingingBallPrefab;

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
    public float tallObstacleYOffset = -0.3f;

    [Header("Level 2 Settings")]
    public float swingingBallInterval = 8f;
    public bool swingingBallActive = false;

    [Header("Difficulty Scaling")]
    public float minPossibleInterval = 1.2f;
    public float maxSpeedMultiplier = 3f;
    public int scorePerDifficultyLevel = 5;

    float timer;
    float nextSpawn;
    float swingingBallTimer = 0f;
    int lastScore = 0;

    void Start() => ScheduleNextSpawn();

    void Update()
    {
        if (GameManager.Instance.IsGameOver()) return;
        if (GameManager.Instance.IsPaused()) return;
        if (LevelManager.Instance != null && LevelManager.Instance.IsLevelComplete()) return;

        timer += Time.deltaTime;

        if (timer >= nextSpawn)
        {
            SpawnObstacle();
            timer = 0f;
            ScheduleNextSpawn();
        }

        // Swinging ball timer for level 2+
        if (swingingBallActive && swingingBallPrefab != null)
        {
            swingingBallTimer += Time.deltaTime;
            if (swingingBallTimer >= swingingBallInterval)
            {
                swingingBallTimer = 0f;
                SpawnSwingingBall();
            }
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

        if (roll < springChance && springPrefab != null)
        {
            Vector3 pos = new Vector3(spawnX, GetGroundY(springPrefab), 0);
            GameObject spring = Instantiate(springPrefab, pos, Quaternion.identity);
            spring.tag = "Spring";
            spring.AddComponent<ObstacleMover>();
        }
        else if (roll < springChance + hangingTopChance && hangingTopPrefab != null)
        {
            Vector3 pos = new Vector3(spawnX, GetTopY(hangingTopPrefab), 0);
            GameObject obs = Instantiate(hangingTopPrefab, pos, Quaternion.identity);
            obs.tag = "DeadlyObstacle";
            obs.AddComponent<ObstacleMover>();
        }
        else if (roll < springChance + hangingTopChance + fallingTopChance
                 && fallingTopPrefab != null)
        {
            Vector3 pos = new Vector3(spawnX * 0.6f, GetTopY(fallingTopPrefab), 0);
            GameObject obs = Instantiate(fallingTopPrefab, pos, Quaternion.identity);
            obs.tag = "DeadlyObstacle";
        }
        else if (roll < springChance + hangingTopChance + fallingTopChance
                 + hangingPoopChance && hangingPoopPrefab != null)
        {
            float y = GetGroundY(hangingPoopPrefab) + tallObstacleYOffset;
            Vector3 pos = new Vector3(spawnX, y, 0);
            GameObject obs = Instantiate(hangingPoopPrefab, pos, Quaternion.identity);
            obs.tag = "DeadlyObstacle";
            obs.AddComponent<ObstacleMover>();
        }
        else if (groundPoopPrefab != null)
        {
            Vector3 pos = new Vector3(spawnX, GetGroundY(groundPoopPrefab), 0);
            GameObject obs = Instantiate(groundPoopPrefab, pos, Quaternion.identity);
            obs.tag = "Obstacle";
            obs.AddComponent<ObstacleMover>();
        }
    }

void SpawnSwingingBall()
{
    if (swingingBallPrefab == null) return;

    // Spawn at mid height so it oscillates through play area
    float midY = 0f;
    Vector3 pos = new Vector3(spawnX + 3f, midY, 0);
    GameObject ball = Instantiate(swingingBallPrefab, pos, Quaternion.identity);
    ball.tag = "DeadlyObstacle";
    Debug.Log("Swinging ball spawned!");
}

    public void SetLevel(int level)
    {
        Debug.Log("Setting level: " + level);

        if (level == 1)
        {
            minSpawnInterval = 1.5f;
            maxSpawnInterval = 3.5f;
            springChance = 0.2f;
            hangingTopChance = 0.1f;
            fallingTopChance = 0.1f;
            hangingPoopChance = 0.1f;
            swingingBallActive = false;
        }
        else if (level == 2)
        {
            minSpawnInterval = 1.2f;
            maxSpawnInterval = 3f;
            springChance = 0.15f;
            hangingTopChance = 0.15f;
            fallingTopChance = 0.15f;
            hangingPoopChance = 0.15f;
            swingingBallActive = true;
            swingingBallTimer = 0f;
            SpawnSwingingBall();
        }
        else if (level >= 3)
        {
            minSpawnInterval = 0.8f;
            maxSpawnInterval = 2.5f;
            springChance = 0.15f;
            hangingTopChance = 0.2f;
            fallingTopChance = 0.2f;
            hangingPoopChance = 0.2f;
            swingingBallActive = true;
            swingingBallInterval = 6f;
        }
    }

    float GetGroundY(GameObject prefab)
    {
        SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return groundY;

        float spriteH = sr.sprite.bounds.size.y;
        Vector3 scale = prefab.transform.localScale;
        float worldHeight = spriteH * scale.y;
        float halfHeight = worldHeight * 0.5f;

        return groundY + halfHeight;
    }

    float GetTopY(GameObject prefab)
    {
        SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
        if (sr == null) return hangingTopY;

        float halfHeight = sr.bounds.size.y * prefab.transform.localScale.y * 0.5f;
        return hangingTopY - halfHeight;
    }
}