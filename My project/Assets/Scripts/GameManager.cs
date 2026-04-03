using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float baseSpeed = 5f;
    public float speedIncreaseRate = 0.1f;
    public float speedPenaltyPerHit = 0.15f;
    public int maxHits = 10;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public Slider poopMeter;
    public GameObject gameOverPanel;

    private float currentSpeed;
    private int score;
    private int hitCount;
    private bool isGameOver;

    void Awake() => Instance = this;

    void Start()
    {
        currentSpeed = baseSpeed;
        score = 0;
        hitCount = 0;
        isGameOver = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (poopMeter != null)
        {
            poopMeter.maxValue = maxHits;
            poopMeter.value = 0;
        }
    }

    void Update()
    {
        if (isGameOver) return;
        currentSpeed += speedIncreaseRate * Time.deltaTime;

        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public float GetCurrentSpeed() => currentSpeed;
    public int GetScore() => score;

    public void AddScore()
    {
        score++;
        Debug.Log("Score: " + score);
    }

    public void PlayerHit()
    {
        hitCount++;
        Debug.Log("Player hit! Count: " + hitCount);

        if (poopMeter != null)
            poopMeter.value = hitCount;

        currentSpeed *= (1f - speedPenaltyPerHit);

        if (hitCount >= maxHits)
            TriggerGameOver();
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER!");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game!");
        Time.timeScale = 1f;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name);
    }

    public bool IsGameOver() => isGameOver;
}

