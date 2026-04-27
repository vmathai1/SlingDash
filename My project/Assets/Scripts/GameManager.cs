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
    public TMP_Text finalTotalScoreText;
    public TMP_Text bestScoreText;

    [Header("Collectible UI")]
    public TMP_Text starsText;
    public TMP_Text diamondsText;
    public TMP_Text finalStarsText;
    public TMP_Text finalDiamondsText;

    [Header("Start Menu")]
    public GameObject startPanel;
    public GameObject instructionsPanel;
    public TMP_Text menuBestScoreText;
    public TMP_Text menuTotalStarsText;
    public TMP_Text menuTotalDiamondsText;

    [Header("Pause UI")]
    public GameObject pauseButton;
    public GameObject pausePanel;

    [Header("Speed Settings")]
    public float baseSpeed = 4f;
    public float maxSpeed = 10f;
    public float boostAmount = 3f;
    public float minSpeed = 0f;

    [Header("Boost Settings")]
    public float maxBoostSpeed = 15f;
    public float boostRampTime = 2f;
    public bool IsGameStarted() => gameStarted;

    float worldSpeed = 4f;
    float score = 0f;
    bool isGameOver = false;
    bool gameStarted = false;
    bool isHoldBoosting = false;
    bool isPaused = false;

    int sessionStars = 0;
    int sessionDiamonds = 0;
    int obstacleScore = 0;
    int collectibleScore = 0;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 0f;

        if (liveScoreText != null)
            liveScoreText.text = "Score: 0";
        if (starsText != null)
            starsText.text = "STARS : 0";
        if (diamondsText != null)
            diamondsText.text = "DIAMONDS : 0";

        if (startPanel != null)
            startPanel.SetActive(true);
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(false);

        LoadMenuStats();
    }

    void LoadMenuStats()
    {
        int best = PlayerPrefs.GetInt("BestScore", 0);
        int stars = PlayerPrefs.GetInt("TotalStars", 0);
        int diamonds = PlayerPrefs.GetInt("TotalDiamonds", 0);

        if (menuBestScoreText != null)
            menuBestScoreText.text = "Best Score: " + best;
        if (menuTotalStarsText != null)
            menuTotalStarsText.text = "STARS : " + stars;
        if (menuTotalDiamondsText != null)
            menuTotalDiamondsText.text = "DIAMONDS : " + diamonds;
    }

    void Update()
    {
        if (isGameOver) return;
        if (!gameStarted) return;
        if (isPaused) return;

        if (!isHoldBoosting && worldSpeed > baseSpeed)
            worldSpeed = Mathf.MoveTowards(worldSpeed, baseSpeed,
                         Time.deltaTime * 3f);

        if (worldSpeed < baseSpeed)
            worldSpeed = Mathf.Min(baseSpeed,
                         worldSpeed + Time.deltaTime * 1.5f);

        baseSpeed = Mathf.Min(maxSpeed,
                    baseSpeed + Time.deltaTime * 0.02f);

        isHoldBoosting = false;

        if (liveScoreText != null)
            liveScoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
    }

    // ── Boost Methods ──

    public void ApplyHoldBoost(float holdTime)
    {
        isHoldBoosting = true;

        float t = Mathf.Clamp01(holdTime / boostRampTime);
        float exponentialT = t * t;

        float targetSpeed = Mathf.Lerp(baseSpeed, maxBoostSpeed, exponentialT);
        worldSpeed = Mathf.MoveTowards(worldSpeed, targetSpeed,
                     Time.deltaTime * maxBoostSpeed);
    }

    // ── Pause Methods ──

    public bool IsPaused() => isPaused;

    public void PauseGame()
    {
        if (isGameOver || !gameStarted) return;
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseButton != null)
            pauseButton.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseButton != null)
            pauseButton.SetActive(true);
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic();
    }

    public void EndGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseButton != null)
            pauseButton.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();

        // Reload scene — Awake shows start panel automatically
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ── Start Menu Methods ──

    public void StartGame()
    {
        gameStarted = true;
        Time.timeScale = 1f;
        isGameOver = false;

        if (startPanel != null)
            startPanel.SetActive(false);
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);

        if (liveScoreText != null)
        {
            liveScoreText.gameObject.SetActive(true);
            liveScoreText.text = "Score: 0";
        }
        if (starsText != null)
        {
            starsText.gameObject.SetActive(true);
            starsText.text = "STARS : 0";
        }
        if (diamondsText != null)
        {
            diamondsText.gameObject.SetActive(true);
            diamondsText.text = "DIAMONDS : 0";
        }

        if (pauseButton != null)
            pauseButton.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic();
    }

    public void ShowInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);
        if (startPanel != null)
            startPanel.SetActive(false);
    }

    public void HideInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
        if (startPanel != null)
            startPanel.SetActive(true);
    }

    // ── Game Methods ──

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

    public void AddObstacleScore(int points)
    {
        score += points;
        obstacleScore += points;

        if (liveScoreText != null)
            liveScoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
    }

    public void AddCollectible(CollectibleItem.CollectibleType type, int points)
    {
        score += points;
        collectibleScore += points;

        if (type == CollectibleItem.CollectibleType.Star)
        {
            sessionStars++;
            int totalStars = PlayerPrefs.GetInt("TotalStars", 0) + 1;
            PlayerPrefs.SetInt("TotalStars", totalStars);

            if (starsText != null)
                starsText.text = "STARS : " + sessionStars;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayStar();
        }
        else if (type == CollectibleItem.CollectibleType.Diamond)
        {
            sessionDiamonds++;
            int totalDiamonds = PlayerPrefs.GetInt("TotalDiamonds", 0) + 1;
            PlayerPrefs.SetInt("TotalDiamonds", totalDiamonds);

            if (diamondsText != null)
                diamondsText.text = "DIAMONDS : " + sessionDiamonds;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayDiamond();
        }

        if (liveScoreText != null)
            liveScoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();

        Debug.Log($"Collected {type} +{points} pts! " +
                  $"Stars: {sessionStars} Diamonds: {sessionDiamonds}");
    }

    public int GetTotalStars() => PlayerPrefs.GetInt("TotalStars", 0);
    public int GetTotalDiamonds() => PlayerPrefs.GetInt("TotalDiamonds", 0);
    public int GetSessionStars() => sessionStars;
    public int GetSessionDiamonds() => sessionDiamonds;

    void SaveAndShowGameOver()
    {
        if (pauseButton != null)
            pauseButton.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(false);

        int finalScore = Mathf.FloorToInt(score);

        int best = PlayerPrefs.GetInt("BestScore", 0);
        if (finalScore > best)
        {
            best = finalScore;
            PlayerPrefs.SetInt("BestScore", best);
        }

        if (finalScoreText != null)
            finalScoreText.text = "Score: " + finalScore;
        if (finalTotalScoreText != null)
            finalTotalScoreText.text = "Total: " + finalScore;
        if (bestScoreText != null)
            bestScoreText.text = "Best: " + best;
        if (finalStarsText != null)
            finalStarsText.text = "STARS : " + sessionStars;
        if (finalDiamondsText != null)
            finalDiamondsText.text = "DIAMONDS : " + sessionDiamonds;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();

        SaveAndShowGameOver();
    }

    public void TriggerExplosionGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        worldSpeed = 0f;
        baseSpeed = 0f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();

        StartCoroutine(ShowGameOverAfterDelay(1.2f));
    }

    System.Collections.IEnumerator ShowGameOverAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SaveAndShowGameOver();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}