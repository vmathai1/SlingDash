using UnityEngine;

public class SwingingBall : MonoBehaviour
{
    [Header("Oscillation Settings")]
    public float oscillateSpeed = 2f;
    public float oscillateRange = 2f;
    public float startY = 0f;

    float worldX;
    float timeOffset;

    void Start()
    {
        worldX = transform.position.x;
        startY = transform.position.y;
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver()) return;
        if (LevelManager.Instance != null &&
            LevelManager.Instance.IsLevelComplete()) return;

        // Move left with world
        float speed = GameManager.Instance.GetCurrentSpeed();
        worldX -= speed * Time.deltaTime;

        // Oscillate vertically
        float offsetY = Mathf.Sin(Time.time * oscillateSpeed + timeOffset)
                        * oscillateRange;

        transform.position = new Vector3(worldX, startY + offsetY, 0);

        if (worldX < -15f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Tire") ||
            col.gameObject.CompareTag("Player"))
        {
            if (!GameManager.Instance.IsGameOver())
            {
                // Let PlayerController handle explosion
                // It listens for DeadlyObstacle tag
                // which is already set on this object
            }
        }
    }
}