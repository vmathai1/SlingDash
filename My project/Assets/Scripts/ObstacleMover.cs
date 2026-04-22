using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    float despawnX = -15f;
    bool pointAwarded = false;
    float tireX = -2f; // tire is locked at X -2

    void Update()
    {
        float speed = GameManager.Instance.GetCurrentSpeed();
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Award 1 point when obstacle passes the tire
        if (!pointAwarded && transform.position.x < tireX)
        {
            pointAwarded = true;
            GameManager.Instance.AddObstacleScore(1);
            Debug.Log("Obstacle passed! +1 point");
        }

        if (transform.position.x < despawnX)
            Destroy(gameObject);
    }
}