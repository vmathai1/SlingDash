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
    bool isBoosting = false;
    float boostHoldTime = 0f;

    bool isBouncing = false;
    float bounceTimer = 0f;
    float maxBounceTime = 2f;

    Canvas mainCanvas;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        jumpsLeft = maxJumps;
        mainCanvas = FindObjectOfType<Canvas>();

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
        if (GameManager.Instance.IsGameOver()) return;
        if (GameManager.Instance.IsPaused()) return;

        HandleInput();
        HandleBoostButton();
        BounceInsideScreen();
        RotateTire();
        HandleFlash();
        HandleBounceTimeout();

        if (isBoosting)
        {
            boostHoldTime += Time.deltaTime;
            GameManager.Instance.ApplyHoldBoost(boostHoldTime);
            TriggerFireIfGrounded();
        }
    }

    void HandleBoostButton()
    {
        if (BoostButton.IsPressed)
        {
            isBoosting = true;
            boostHoldTime = BoostButton.HoldTime;
            GameManager.Instance.ApplyHoldBoost(boostHoldTime);
            TriggerFireIfGrounded();
        }
        else if (!Input.GetKey(KeyCode.RightArrow) && isBoosting)
        {
            bool touchBoosting = false;
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Moved ||
                    Input.GetTouch(i).phase == TouchPhase.Stationary)
                {
                    touchBoosting = true;
                    break;
                }
            }
            if (!touchBoosting && !BoostButton.IsPressed)
            {
                isBoosting = false;
                boostHoldTime = 0f;
            }
        }
    }

    void HandleInput()
    {
        // ── KEYBOARD ──
        if (Input.GetKey(KeyCode.RightArrow))
            isBoosting = true;

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            isBoosting = false;
            boostHoldTime = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.UpArrow))
            TryJump();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            GameManager.Instance.SlowDown(2f);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            StopBounce();

        // ── TOUCH ──
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // Skip touches on boost button area only
            if (IsTouchOnBoostButton(touch.position)) continue;

            if (touch.phase == TouchPhase.Began)
            {
                swipeStart = touch.position;
                swipeConsumed = false;
            }

            if (touch.phase == TouchPhase.Moved && !swipeConsumed)
            {
                Vector2 swipeDelta = touch.position - swipeStart;
                float absX = Mathf.Abs(swipeDelta.x);
                float absY = Mathf.Abs(swipeDelta.y);

                if (swipeDelta.x < -swipeThreshold && absY < 80f)
                {
                    GameManager.Instance.SlowDown(2f);
                    swipeConsumed = true;
                }
                else if (swipeDelta.y < -swipeThreshold && absX < 80f)
                {
                    StopBounce();
                    swipeConsumed = true;
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                Vector2 swipeDelta = touch.position - swipeStart;
                float absX = Mathf.Abs(swipeDelta.x);
                float absY = Mathf.Abs(swipeDelta.y);

                if (absX < swipeThreshold && absY < swipeThreshold && !swipeConsumed)
                    TryJump();
                else if (swipeDelta.y > swipeThreshold && absX < absY && !swipeConsumed)
                    TryJump();

                swipeConsumed = false;
            }
        }

        // No touches and no keyboard — stop boost
        if (Input.touchCount == 0 && !Input.GetKey(KeyCode.RightArrow)
            && !BoostButton.IsPressed)
        {
            isBoosting = false;
            boostHoldTime = 0f;
        }
    }

    bool IsTouchOnBoostButton(Vector2 screenPos)
    {
        // Only block touches physically on the button area
        float screenW = Screen.width;
        float screenH = Screen.height;

        float scaleFactor = mainCanvas != null ? mainCanvas.scaleFactor : 1f;

        float buttonW = 250f * scaleFactor;
        float buttonH = 230f * scaleFactor;
        float buttonCenterX = screenW - (200f * scaleFactor);
        float buttonCenterY = 200f * scaleFactor;

        float left = buttonCenterX - buttonW / 2f;
        float right = buttonCenterX + buttonW / 2f;
        float bottom = buttonCenterY - buttonH / 2f;
        float top = buttonCenterY + buttonH / 2f;

        return screenPos.x > left &&
               screenPos.x < right &&
               screenPos.y > bottom &&
               screenPos.y < top;
    }

    void TryJump()
    {
        if (jumpsLeft <= 0) return;

        float topY = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        if (transform.position.y > topY * 0.4f) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpsLeft--;
        isBouncing = true;
        bounceTimer = 0f;
    }

    void StopBounce()
    {
        if (isBouncing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.gravityScale = 3f;
            isBouncing = false;
            bounceTimer = 0f;
        }
    }

    void HandleBounceTimeout()
    {
        if (isBouncing)
        {
            bounceTimer += Time.deltaTime;
            if (bounceTimer >= maxBounceTime)
            {
                isBouncing = false;
                bounceTimer = 0f;
            }
        }
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

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayBounce();
        }

        // Bottom bounce
        if (pos.y - radius <= bottomY)
        {
            pos.y = bottomY + radius;
            vel.y = Mathf.Abs(vel.y) * 0.8f;
            rb.gravityScale = 2f;
        }

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

        sr.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayKill();

        if (explosionEffect != null)
        {
            Debug.Log("Playing explosion!");
            explosionEffect.Play();
        }
        else
            Debug.LogError("explosionEffect is null!");

        GameManager.Instance.TriggerExplosionGameOver();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            jumpsLeft = maxJumps;
            isBouncing = false;
            bounceTimer = 0f;
            rb.gravityScale = 2f;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayBounce();
        }

        if (col.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.SlowDown(GameManager.Instance.WorldSpeed);
            isFlashing = true;
            flashTimer = 0f;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayHit();
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
        if (col.gameObject.CompareTag("ForegroundTop"))
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                -Mathf.Abs(rb.linearVelocity.y) * 0.6f);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayBounce();
        }

        if (col.gameObject.CompareTag("Spring"))
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce * 2f);
            jumpsLeft = maxJumps;
            isBouncing = true;
            bounceTimer = 0f;
            GameManager.Instance.BoostForward();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayBounce();

            SpringEffect spring = col.GetComponent<SpringEffect>();
            if (spring != null) spring.Compress();

            Debug.Log("Spring hit! Launching!");
        }

        if (col.gameObject.CompareTag("DeadlyObstacle"))
            TriggerExplosion();
    }
}