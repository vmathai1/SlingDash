using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    float despawnX = -15f;

    void Update()
    {
        // Keep moving regardless of game over
        // so obstacles don't freeze and trap the tire
        float speed = GameManager.Instance.GetCurrentSpeed();
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x < despawnX)
            Destroy(gameObject);
    }
}