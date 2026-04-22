using UnityEngine;

public class FallingObstacle : MonoBehaviour
{
    [SerializeField] float minHangTime = 0.3f;
    [SerializeField] float maxHangTime = 1f;
    [SerializeField] float fallSpeed = 15f;
    [SerializeField] float warningFlashRate = 0.08f;
    [SerializeField] float warningDuration = 0.4f;

    enum State { Hanging, Warning, Falling, Dead }
    State state = State.Hanging;

    float timer = 0f;
    float flashTimer = 0f;
    float hangTime;
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        hangTime = Random.Range(minHangTime, maxHangTime);
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver()) return;

        // Move left with world
        float worldSpeed = GameManager.Instance.GetCurrentSpeed();
        transform.Translate(Vector2.left * worldSpeed * Time.deltaTime);

        // Despawn off screen left
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
            return;
        }

        switch (state)
        {
            case State.Hanging:
                timer += Time.deltaTime;
                if (timer >= hangTime)
                {
                    state = State.Warning;
                    timer = 0f;
                }
                break;

            case State.Warning:
                // Flash red to warn player
                flashTimer += Time.deltaTime;
                if (flashTimer >= warningFlashRate)
                {
                    flashTimer = 0f;
                    sr.color = sr.color == Color.red
                        ? Color.white : Color.red;
                }
                timer += Time.deltaTime;
                if (timer >= warningDuration)
                {
                    state = State.Falling;
                    sr.color = Color.white;
                    timer = 0f;
                }
                break;

            case State.Falling:
                transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

                if (transform.position.y < -10f)
                    Destroy(gameObject);
                break;

            case State.Dead:
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            state = State.Dead;
            Destroy(gameObject, 0.3f);
        }
    }
}