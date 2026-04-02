using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    private float speed;
    private bool isHanging;
    private bool isDroppingDown;
    public float dropSpeed = 8f;
    private Transform player;

    void Start()
    {
        isHanging = CompareTag("HangingPoop");
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        speed = GameManager.Instance.GetCurrentSpeed();

        // Always move left
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Drop when directly above player
        if (isHanging && player != null && !isDroppingDown)
        {
            float distanceX = Mathf.Abs(
                transform.position.x - player.position.x);
            if (distanceX < 1f)
                isDroppingDown = true;
        }

        // Drop down
        if (isDroppingDown)
            transform.Translate(Vector2.down * dropSpeed * Time.deltaTime);

        // Destroy when off screen or fallen too low
        if (transform.position.x < -15f || transform.position.y < -10f)
            Destroy(gameObject);
    }
}

