using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public enum CollectibleType { Star, Diamond }

    [Header("Settings")]
    public CollectibleType type = CollectibleType.Star;
    public int pointValue = 10;
    public float bobSpeed = 2f;
    public float bobHeight = 0.3f;

    Vector3 startPos;
    float bobTimer;

    void Start()
    {
        startPos = transform.position;
        bobTimer = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver()) return;

        // Move left with world
        float speed = GameManager.Instance.GetCurrentSpeed();
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Bob up and down
        bobTimer += Time.deltaTime * bobSpeed;
        transform.position = new Vector3(
            transform.position.x,
            startPos.y + Mathf.Sin(bobTimer) * bobHeight,
            transform.position.z
        );

        // Despawn off screen
        if (transform.position.x < -15f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") ||
            col.CompareTag("Tire"))
        {
            Collect();
        }
    }

    void Collect()
    {
        GameManager.Instance.AddCollectible(type, pointValue);
        Destroy(gameObject);
    }
}