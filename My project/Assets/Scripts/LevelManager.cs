using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int[] levelScoreThresholds = { 200, 500, 1000 };

    [Header("UI")]
    public GameObject levelCompletePanel;
    public TMP_Text levelCompleteText;
    public TMP_Text nextLevelText;
    public TMP_Text currentLevelText;

    bool levelComplete = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (levelComplete) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver()) return;
        if (!GameManager.Instance.IsGameStarted()) return;

        // Check if score threshold reached
        int score = Mathf.FloorToInt(GameManager.Instance.GetScore());
        int threshold = GetCurrentThreshold();

        if (score >= threshold)
            TriggerLevelComplete();

        // Update level display
        if (currentLevelText != null)
            currentLevelText.text = "LVL " + currentLevel;
    }

    int GetCurrentThreshold()
    {
        if (currentLevel - 1 < levelScoreThresholds.Length)
            return levelScoreThresholds[currentLevel - 1];
        return int.MaxValue;
    }

    void TriggerLevelComplete()
    {
        levelComplete = true;

        // Slow world to a stop
        GameManager.Instance.SlowDown(100f);
        Time.timeScale = 0.3f;

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        if (levelCompleteText != null)
            levelCompleteText.text = "LEVEL " + currentLevel + "\nCOMPLETE!";

        if (nextLevelText != null)
            nextLevelText.text = "Level " + (currentLevel + 1) + " incoming...";

        // Auto advance after 2 seconds
        StartCoroutine(AdvanceLevel());
    }

    System.Collections.IEnumerator AdvanceLevel()
    {
        yield return new WaitForSecondsRealtime(2.5f);

        currentLevel++;
        levelComplete = false;
        Time.timeScale = 1f;

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        // Tell spawner to update difficulty
        ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
        if (spawner != null)
            spawner.SetLevel(currentLevel);

        Debug.Log("Advanced to Level " + currentLevel);
    }

    public int GetCurrentLevel() => currentLevel;
    public bool IsLevelComplete() => levelComplete;
}