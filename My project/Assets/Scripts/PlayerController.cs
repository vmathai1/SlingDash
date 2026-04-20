using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 14f;
    [SerializeField] int maxJumps = 2;

    [Header("Swipe Settings")]
    [SerializeField] float swipeThreshold = 30f;
    [SerializeField] float swipeSlowdown = 2f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Camera cam;
    int jumpsLeft;
    float flashTimer = 0f;
    bool isFlashing = false;

    // Swipe tracking
    Vector2 swipeStart;
    bool swipeConsumed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver()) return;

        HandleInput();
        BounceInsideScreen();
        RotateTire();
        HandleFlash();
    }

    void HandleInput()
    {
        // ── TOUCH ──
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                swipeStart = touch.position;
                swipeConsumed = false;
            }

            if (touch.phase == TouchPhase.Ended && !swipeConsumed)
            {
                Vector2 swipeDelta = touch.position - swipeStart;
                float absX = Mathf.Abs(swipeDelta.x);
                float absY = Mathf.Abs(swipeDelta.y);

                if (absX < swipeThreshold && absY < swipeThreshold)
                {
                    // Tap — jump
                    TryJump();
                }
                else if (absX > absY)
                {
                    if (swipeDelta.x > swipeThreshold)
                    {
                        // Swipe right — boost forward
                        GameManager.Instance.BoostForward();
                        swipeConsumed = true;
                    }
                    else if (swipeDelta.x < -swipeThreshold)
                    {
                        // Swipe left — slow down
                        GameManager.Instance.SlowDown(swipeSlowdown);
                        swipeConsumed = true;
                    }
                }
                else if (swipeDelta.y > swipeThreshold)
                {
                    // Swipe up — jump
                    TryJump();
                    swipeConsumed = true;
                }
            }
        }

        // ── KEYBOARD (editor testing) ──
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            TryJump();
        if (Input.GetKeyDown(KeyCode.RightArrow))
            GameManager.Instance.BoostForward();
        if (Input.GetKey(KeyCode.LeftArrow))
            GameManager.Instance.SlowDown(Time.deltaTime * swipeSlowdown);
    }

    void TryJump()
    {
        if (jumpsLeft <= 0) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpsLeft--;
    }

    void BounceInsideScreen()
    {
        float topY = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        float bottomY = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;

        float radius = GetComponent<CircleCollider2D>().radius
                       * transform.localScale.y;
        Vector3 pos = transform.position;
        Vector2 vel = rb.linearVelocity;

        if (pos.y + radius >= topY)
        {
            pos.y = topY - radius;
            vel.y = -Mathf.Abs(vel.y) * 0.6f;
        }

        if (pos.y - radius <= bottomY)
        {
            pos.y = bottomY + radius;
            vel.y = Mathf.Abs(vel.y) * 0.6f;
        }

        pos.x = Mathf.Clamp(pos.x,
                cam.ViewportToWorldPoint(Vector3.zero).x + radius,
                cam.ViewportToWorldPoint(Vector3.right).x - radius);
        vel.x = 0f;

        transform.position = pos;
        rb.linearVelocity = vel;
    }

    void RotateTire()
    {
        float worldSpeed = GameManager.Instance.WorldSpeed;
        transform.Rotate(0f, 0f,
            -worldSpeed * 3f * Time.deltaTime * Mathf.Rad2Deg * 0.5f);
    }

    void HandleFlash()
    {
        if (isFlashing)
        {
            flashTimer += Time.deltaTime;
            sr.color = Mathf.Sin(flashTimer * 20f) > 0
                ? Color.red : Color.white;
            if (flashTimer > 1f)
            {
                isFlashing = false;
                flashTimer = 0f;
                sr.color = Color.white;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            jumpsLeft = maxJumps;

        if (col.gameObject.CompareTag("Obstacle"))
        {
            // Slow world to a stop on hit
            GameManager.Instance.SlowDown(GameManager.Instance.WorldSpeed);
            isFlashing = true;
            flashTimer = 0f;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Obstacle"))
        {
            // Give a small boost when escaping obstacle
            GameManager.Instance.BoostForward();
        }
    }
}