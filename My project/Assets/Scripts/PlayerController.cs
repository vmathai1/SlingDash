using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 14f;
    [SerializeField] int maxJumps = 2;

    [Header("Swipe Settings")]
    [SerializeField] float swipeSlowdown = 0.015f;
    [SerializeField] float minSpeed = 1.5f;

    Rigidbody2D rb;
    int jumpsLeft;
    bool isDead;

    // Swipe tracking
    Vector2 swipeStart;
    bool trackingSwipe;

    // Public so GameManager can read it
    public bool IsDead => isDead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        if (isDead) return;
        HandleInput();
        RotateTire();
    }

    void HandleInput()
    {
        // Touch input (device)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                swipeStart = touch.position;
                trackingSwipe = true;
            }

            if (touch.phase == TouchPhase.Moved && trackingSwipe)
            {
                float dx = touch.position.x - swipeStart.x;
                if (dx < -30f) // swiping left
                {
                    float pullAmount = Mathf.Abs(dx) / 300f;
                    // Slows world speed via GameManager
                    GameManager.Instance.SlowDown(pullAmount * swipeSlowdown);
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                float dx = touch.position.x - swipeStart.x;
                float dy = Mathf.Abs(touch.position.y - swipeStart.y);
                // Only jump if it wasn't a left swipe
                if (dx > -30f && dy < 50f)
                    TryJump();
                trackingSwipe = false;
            }
        }

        // Keyboard fallback (editor testing)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            TryJump();
        if (Input.GetKey(KeyCode.LeftArrow))
            GameManager.Instance.SlowDown(swipeSlowdown);
    }

    void TryJump()
    {
        if (jumpsLeft <= 0) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpsLeft--;
    }

    void RotateTire()
    {
        // Rotate clockwise based on world scroll speed
        float worldSpeed = GameManager.Instance.WorldSpeed;
        transform.Rotate(0f, 0f, -worldSpeed * 3f * Time.deltaTime * Mathf.Rad2Deg * 0.5f);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            jumpsLeft = maxJumps;
        }

        if (col.gameObject.CompareTag("Obstacle"))
        {
            isDead = true;
            rb.linearVelocity = new Vector2(-3f, 6f); // knock back on hit
            GameManager.Instance.TriggerGameOver();
        }
    }
}