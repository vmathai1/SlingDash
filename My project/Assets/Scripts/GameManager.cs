using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject gameOverPanel;
    public TMP_Text liveScoreText;
    public TMP_Text finalScoreText;

    [Header("Speed Settings")]
    public float baseSpeed = 4f;
    public float maxSpeed = 10f;
    public float boostAmount = 3f;
    public float minSpeed = 0f;

    float worldSpeed = 4f;
    float score = 0f;
    bool isGameOver = false;

    void Awake() => Instance = this;

    void Update()
    {
        if (isGameOver) return;

        score += Time.deltaTime * 10f;

        // Gradually recover speed back to base over time
        if (worldSpeed < baseSpeed)
            worldSpeed = Mathf.Min(baseSpeed,
                         worldSpeed + Time.deltaTime * 1.5f);

        // Gradually increase base speed over time
        baseSpeed = Mathf.Min(maxSpeed,
                    baseSpeed + Time.deltaTime * 0.02f);

        if (liveScoreText != null)
            liveScoreText.text = Mathf.FloorToInt(score).ToString();
    }

    public float WorldSpeed => worldSpeed;
    public float GetCurrentSpeed() => worldSpeed;
    public bool IsGameOver() => isGameOver;
    public float GetScore() => score;

    public void SlowDown(float amount)
    {
        worldSpeed = Mathf.Max(minSpeed, worldSpeed - amount);
        baseSpeed = Mathf.Max(2f, baseSpeed - amount * 0.5f);
    }

    public void BoostForward()
    {
        worldSpeed = Mathf.Min(maxSpeed, worldSpeed + boostAmount);
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (finalScoreText != null)
            finalScoreText.text = "Score: " + Mathf.FloorToInt(score);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        int best = PlayerPrefs.GetInt("BestScore", 0);
        if (Mathf.FloorToInt(score) > best)
            PlayerPrefs.SetInt("BestScore", Mathf.FloorToInt(score));
    }

    public void TriggerExplosionGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Stop world immediately
        worldSpeed = 0f;
        baseSpeed = 0f;

        int best = PlayerPrefs.GetInt("BestScore", 0);
        if (Mathf.FloorToInt(score) > best)
            PlayerPrefs.SetInt("BestScore", Mathf.FloorToInt(score));

        // Delay showing UI so explosion plays first
        StartCoroutine(ShowGameOverAfterDelay(1.2f));
    }

    System.Collections.IEnumerator ShowGameOverAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (finalScoreText != null)
            finalScoreText.text = "Score: " + Mathf.FloorToInt(score);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}