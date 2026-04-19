using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    float worldSpeed = 4f;
    float score = 0f;
    bool isGameOver = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isGameOver) return;
        score += Time.deltaTime * 10f;
        // Gradually speed up over time
        worldSpeed = Mathf.Min(10f, worldSpeed + Time.deltaTime * 0.05f);
    }

    // Used by PlayerController
    public float WorldSpeed => worldSpeed;

    // Used by ObstacleMover and BGScroller
    public float GetCurrentSpeed() => worldSpeed;

    // Used by ObstacleSpawner and BGScroller
    public bool IsGameOver() => isGameOver;

    // Used by ObstacleSpawner
    public float GetScore() => score;

    // Used by PlayerController
    public void SlowDown(float amount)
    {
        worldSpeed = Mathf.Max(1.5f, worldSpeed - amount);
    }

    // Used by PlayerController
    public void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over! Score: " + Mathf.FloorToInt(score));
    }
}