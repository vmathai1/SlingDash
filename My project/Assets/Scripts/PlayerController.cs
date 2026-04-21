using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 14f;
    [SerializeField] int maxJumps = 2;

    [Header("Swipe Settings")]
    [SerializeField] float swipeThreshold = 30f;
    [SerializeField] float swipeSlowdown = 2f;

    [Header("Effects")]
    public ExplosionEffect explosionEffect;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Camera cam;
    FireEffect fireEffect;
    int jumpsLeft;
    float flashTimer = 0f;
    bool isFlashing = false;

    Vector2 swipeStart;
    bool swipeConsumed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        jumpsLeft = maxJumps;

        fireEffect = GetComponentInChildren<FireEffect>(true);
        if (fireEffect == null)
            fireEffect = FindObjectOfType<FireEffect>();

        if (fireEffect == null)
            Debug.LogError("FireEffect NOT FOUND on Tire!");
        else
            Debug.Log("FireEffect found: " + fireEffect.gameObject.name);

        if (explosionEffect == null)
            Debug.LogError("ExplosionEffect NOT assigned in Inspector!");
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
                    TryJump();
                }
                else if (absX > absY)
                {
                    if (swipeDelta.x > swipeThreshold)
                    {
                        GameManager.Instance.BoostForward();
                        TriggerFireIfGrounded();
                        swipeConsumed = true;
                    }
                    else if (swipeDelta.x < -swipeThreshold)
                    {
                        GameManager.Instance.SlowDown(2f);
                        swipeConsumed = true;
                    }
                }
                else if (swipeDelta.y > swipeThreshold)
                {
                    TryJump();
                    swipeConsumed = true;
                }
            }
        }

        // ── KEYBOARD ──
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            TryJump();
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameManager.Instance.BoostForward();
            TriggerFireIfGrounded();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            GameManager.Instance.SlowDown(2f);
    }

    void TryJump()
    {
        if (jumpsLeft <= 0) return;

        float topY = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        if (transform.position.y > topY * 0.4f) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpsLeft--;
    }

    void TriggerFireIfGrounded()
    {
        float radius = GetComponent<CircleCollider2D>().radius
                       * transform.localScale.y;
        Vector2 checkPos = new Vector2(
            transform.position.x,
            transform.position.y - radius - 0.1f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, 0.2f);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.CompareTag("Ground"))
            {
                if (fireEffect != null)
                    fireEffect.TriggerFire();
                return;
            }
        }
    }

    void BounceInsideScreen()
    {
        float topY = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        float bottomY = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;

        float radius = GetComponent<CircleCollider2D>().radius
                       * transform.localScale.y;
        Vector3 pos = transform.position;
        Vector2 vel = rb.linearVelocity;

        // Top bounce
        if (pos.y + radius >= topY)
        {
            pos.y = topY - radius;
            vel.y = -Mathf.Abs(vel.y) * 0.8f;
        }

        // Bottom bounce
        if (pos.y - radius <= bottomY)
        {
            pos.y = bottomY + radius;
            vel.y = Mathf.Abs(vel.y) * 0.8f;
        }

        // Lock tire to fixed X position
        pos.x = -2f;
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

    void TriggerExplosion()
    {
        if (GameManager.Instance.IsGameOver()) return;

        Debug.Log("TriggerExplosion called!");

        // Hide tire
        sr.enabled = false;

        // Stop tire physics
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        // Play explosion animation
        if (explosionEffect != null)
        {
            Debug.Log("Playing explosion!");
            explosionEffect.Play();
        }
        else
        {
            Debug.LogError("explosionEffect is null!");
        }

        // Trigger game over with delay
        GameManager.Instance.TriggerExplosionGameOver();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            jumpsLeft = maxJumps;

        if (col.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.SlowDown(GameManager.Instance.WorldSpeed);
            isFlashing = true;
            flashTimer = 0f;
        }

        if (col.gameObject.CompareTag("DeadlyObstacle"))
            TriggerExplosion();
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Obstacle"))
            GameManager.Instance.BoostForward();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Bounce off top foreground
        if (col.gameObject.CompareTag("ForegroundTop"))
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                -Mathf.Abs(rb.linearVelocity.y) * 0.6f);
        }

        // Spring — launch tire upward at 2x jump force
        if (col.gameObject.CompareTag("Spring"))
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce * 2f);
            jumpsLeft = maxJumps;
            GameManager.Instance.BoostForward();

            SpringEffect spring = col.GetComponent<SpringEffect>();
            if (spring != null) spring.Compress();

            Debug.Log("Spring hit! Launching!");
        }

        // Deadly obstacle trigger
        if (col.gameObject.CompareTag("DeadlyObstacle"))
            TriggerExplosion();
    }
}