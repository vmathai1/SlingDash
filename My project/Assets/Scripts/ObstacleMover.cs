using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    float despawnX = -15f;

    void Update()
    {
        if (GameManager.Instance.IsGameOver()) return;

        float speed = GameManager.Instance.GetCurrentSpeed();
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x < despawnX)
            Destroy(gameObject);
    }
}