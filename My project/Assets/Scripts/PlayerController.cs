using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 12f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public float hitDistance = 1.5f;
    public int maxJumps = 3;
    public float maxJumpHeight = 3f;
    public float duckScaleY = 0.65f;
    public float jumpCooldown = 0.3f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool isDucking;
    private int jumpCount = 0;
    private int hitCount;
    private float scoreTimer;
    private float scoreInterval = 1.5f;
    private Vector3 originalScale;
    private float duckTimer;
    private float lastJumpTime = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver()) return;

        // Only detect actual ground layer
        isGrounded = Physics2D.Raycast(
            transform.position, Vector2.down, 1.2f, groundLayer);

        // Only reset when truly landed
        if (isGrounded && rb.linearVelocity.y <= 0.05f)
        {
            if (jumpCount > 0)
            jumpCount = 0;
        }

        // Stop at max height
        if (transform.position.y >= maxJumpHeight && rb.linearVelocity.y > 0)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        // Score over time
        scoreTimer += Time.deltaTime;
        if (scoreTimer >= scoreInterval)
        {
            GameManager.Instance.AddScore();
            scoreTimer = 0f;
        }

        // Auto stop ducking
        if (isDucking)
        {
            duckTimer += Time.deltaTime;
            if (duckTimer >= 1f)
            {
                StopDucking();
                duckTimer = 0f;
            }
        }

        CheckPoopCollision();

        // Keyboard jump
        if (Input.GetKeyDown(KeyCode.Space) && !isDucking)
            Jump();

        // Keyboard duck
        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded)
            Duck();
        if (Input.GetKeyUp(KeyCode.DownArrow))
            StopDucking();
    }

    public void Jump()
    {
        // Check cooldown
        if (Time.time - lastJumpTime < jumpCooldown) return;
        // Check max jumps
        if (jumpCount >= maxJumps) return;
        if (isDucking) return;
        if (transform.position.y >= maxJumpHeight) return;

        jumpCount++;
        lastJumpTime = Time.time;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        Debug.Log("Jump " + jumpCount + " of " + maxJumps);
    }

    public void Duck()
    {
        if (!isGrounded) return;
        isDucking = true;
        duckTimer = 0f;
        transform.localScale = new Vector3(
            originalScale.x * 1.2f,
            originalScale.y * duckScaleY,
            originalScale.z);
    }

    public void StopDucking()
    {
        isDucking = false;
        duckTimer = 0f;
        transform.localScale = originalScale;
    }

    void CheckPoopCollision()
    {
        GameObject[] groundPoops =
            GameObject.FindGameObjectsWithTag("GroundPoop");
        GameObject[] hangingPoops =
            GameObject.FindGameObjectsWithTag("HangingPoop");

        foreach (GameObject poop in groundPoops)
        {
            float distance = Vector2.Distance(
                transform.position, poop.transform.position);
            if (distance < hitDistance)
            {
                HandleHit();
                Destroy(poop);
                return;
            }
        }

        foreach (GameObject poop in hangingPoops)
        {
            float distance = Vector2.Distance(
                transform.position, poop.transform.position);
            if (distance < hitDistance && !isDucking)
            {
                HandleHit();
                Destroy(poop);
                return;
            }
        }
    }

    void HandleHit()
    {
        hitCount++;
        GameManager.Instance.PlayerHit();
        transform.localScale = originalScale;
        isDucking = false;

        if (animator != null)
            animator.SetInteger("HitCount", hitCount);

    }
}