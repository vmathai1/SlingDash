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
    public GameObject startPanel;
    public GameObject pausePanel;
    public GameObject pauseButton;

    [Header("Audio")]
    public AudioSource backgroundMusic;
    public AudioClip gameOverClip;
    public AudioClip hitClip;
    public AudioClip scoreClip;

    private float currentSpeed;
    private int score;
    private int hitCount;
    private bool isGameOver;
    private bool isGameStarted;
    private bool isPaused;
    private AudioSource audioSource;

    void Awake() => Instance = this;

    void Start()
    {
        currentSpeed = baseSpeed;
        score = 0;
        hitCount = 0;
        isGameOver = false;
        isGameStarted = false;
        isPaused = false;

        audioSource = GetComponent<AudioSource>();

        // Show start panel hide everything else
        if (startPanel != null)
            startPanel.SetActive(true);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (pauseButton != null)
            pauseButton.SetActive(false);

        // Pause game until play pressed
        Time.timeScale = 0f;

        if (poopMeter != null)
        {
            poopMeter.maxValue = maxHits;
            poopMeter.value = 0;
        }
    }

    void Update()
    {
        if (!isGameStarted) return;
        if (isGameOver) return;
        if (isPaused) return;

        currentSpeed += speedIncreaseRate * Time.deltaTime;

        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void StartGame()
    {
        isGameStarted = true;

        // Hide start panel
        if (startPanel != null)
            startPanel.SetActive(false);

        // Show pause button
        if (pauseButton != null)
            pauseButton.SetActive(true);

        // Start game
        Time.timeScale = 1f;

        // Start music
        if (backgroundMusic != null)
            backgroundMusic.Play();

        Debug.Log("Game Started!");
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
        if (pauseButton != null)
            pauseButton.SetActive(false);

        // Pause music
        if (backgroundMusic != null)
            backgroundMusic.Pause();

        Debug.Log("Game Paused!");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (pauseButton != null)
            pauseButton.SetActive(true);

        // Resume music
        if (backgroundMusic != null)
            backgroundMusic.UnPause();

        Debug.Log("Game Resumed!");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name);
    }

    public float GetCurrentSpeed()
    {
        if (!isGameStarted || isPaused) return 0f;
        return currentSpeed;
    }

    public int GetScore() => score;

    public void AddScore()
    {
        if (!isGameStarted || isPaused) return;
        score++;

        if (scoreClip != null)
            AudioSource.PlayClipAtPoint(
                scoreClip, transform.position, 0.5f);
    }

    public void PlayerHit()
    {
        if (!isGameStarted) return;
        hitCount++;

        if (poopMeter != null)
            poopMeter.value = hitCount;

        if (hitClip != null)
            AudioSource.PlayClipAtPoint(
                hitClip, transform.position, 0.7f);

        currentSpeed *= (1f - speedPenaltyPerHit);

        if (hitCount >= maxHits)
            TriggerGameOver();
    }

    void TriggerGameOver()
    {
        isGameOver = true;

        if (backgroundMusic != null)
            backgroundMusic.Stop();

        if (audioSource != null && gameOverClip != null)
        {
            audioSource.clip = gameOverClip;
            audioSource.loop = false;
            audioSource.Play();
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        if (pauseButton != null)
            pauseButton.SetActive(false);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name);
    }

    public bool IsGameOver() => isGameOver;
    public bool IsGameStarted() => isGameStarted;
    public bool IsPaused() => isPaused;
}