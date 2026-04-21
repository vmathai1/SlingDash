using UnityEngine;

public class BGScroller : MonoBehaviour
{
    [SerializeField] float speedMultiplier = 1f;
    [SerializeField] Transform otherCopy;

    float imageWidth;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("BGScroller needs a SpriteRenderer on: " + gameObject.name);
            enabled = false;
            return;
        }
        imageWidth = sr.bounds.size.x;
    }

    void Update()
    {
        float speed = GameManager.Instance.GetCurrentSpeed() * speedMultiplier;
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x + imageWidth / 2f < -imageWidth / 2f)
        {
            if (otherCopy != null)
                transform.position = new Vector3(
                    otherCopy.position.x + imageWidth,
                    transform.position.y,
                    transform.position.z);
            else
                transform.position = new Vector3(
                    transform.position.x + imageWidth * 2f,
                    transform.position.y,
                    transform.position.z);
        }
    }
}