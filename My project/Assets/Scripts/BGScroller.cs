using UnityEngine;

public class BGScroller : MonoBehaviour
{
    [SerializeField] float speedMultiplier = 1f;
    [SerializeField] Transform otherCopy;
    [SerializeField] float overlapAmount = 0.2f;

    float imageWidth;
    float camHalfWidth;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) { enabled = false; return; }
        imageWidth = sr.bounds.size.x;

        // Get camera half width once
        camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        Debug.Log(gameObject.name + " width = " + imageWidth);
    }

    void Update()
    {
        float speed = GameManager.Instance.GetCurrentSpeed() * speedMultiplier;

        // Move extra distance this frame accounting for speed
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Reset when RIGHT edge goes past camera LEFT edge
        float rightEdge = transform.position.x + imageWidth / 2f;
        float cameraLeftEdge = Camera.main.transform.position.x - camHalfWidth;

        if (rightEdge < cameraLeftEdge)
        {
            if (otherCopy != null)
            {
                float otherRightEdge = otherCopy.position.x + imageWidth / 2f;
                transform.position = new Vector3(
                    otherRightEdge + imageWidth / 2f - overlapAmount,
                    transform.position.y,
                    transform.position.z);
            }
            else
            {
                transform.position = new Vector3(
                    transform.position.x + imageWidth * 2f - overlapAmount,
                    transform.position.y,
                    transform.position.z);
            }
        }
    }
}