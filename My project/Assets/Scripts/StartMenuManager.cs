using UnityEngine;
using TMPro;

public class StartMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject instructionsPanel;

    [Header("Main Menu Texts")]
    public TMP_Text bestScoreText;
    public TMP_Text totalStarsText;
    public TMP_Text totalDiamondsText;

    void Start()
    {
        if (startPanel != null)
            startPanel.SetActive(true);

        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);

        // Pause game until play is pressed
        Time.timeScale = 0f;

        // Load saved stats
        int best = PlayerPrefs.GetInt("BestScore", 0);
        int stars = PlayerPrefs.GetInt("TotalStars", 0);
        int diamonds = PlayerPrefs.GetInt("TotalDiamonds", 0);

        if (bestScoreText != null)
            bestScoreText.text = "Best Score: " + best;

        if (totalStarsText != null)
            totalStarsText.text = "STARS : " + stars;

        if (totalDiamondsText != null)
            totalDiamondsText.text = "DIAMONDS : " + diamonds;
    }

    public void StartGame()
    {
        if (startPanel != null)
            startPanel.SetActive(false);

        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);

        Time.timeScale = 1f;
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
}