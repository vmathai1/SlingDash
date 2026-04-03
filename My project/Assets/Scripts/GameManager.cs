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

    [Header("Audio")]
    public AudioSource backgroundMusic;
    public AudioClip gameOverClip;
    public AudioClip hitClip;
    public AudioClip scoreClip;

    private float currentSpeed;
    private int score;
    private int hitCount;
    private bool isGameOver;
    private AudioSource audioSource;

    void Awake() => Instance = this;

    void Start()
    {
        currentSpeed = baseSpeed;
        score = 0;
        hitCount = 0;
        isGameOver = false;

        audioSource = GetComponent<AudioSource>();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (poopMeter != null)
        {
            poopMeter.maxValue = maxHits;
            poopMeter.value = 0;
        }

        // Start background music
        if (backgroundMusic != null)
            backgroundMusic.Play();
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

        // Play score sound
        if (scoreClip != null)
            AudioSource.PlayClipAtPoint(
                scoreClip, transform.position, 0.5f);
    }

    public void PlayerHit()
    {
        hitCount++;
        Debug.Log("Player hit! Count: " + hitCount);

        if (poopMeter != null)
            poopMeter.value = hitCount;

        // Play hit sound
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
        Debug.Log("GAME OVER!");

        // Stop background music
        if (backgroundMusic != null)
            backgroundMusic.Stop();

        // Play game over music
        if (audioSource != null && gameOverClip != null)
        {
            audioSource.clip = gameOverClip;
            audioSource.loop = false;
            audioSource.Play();
        }

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