using UnityEngine;

public class CameraFit : MonoBehaviour
{
    [SerializeField] bool fitWidth = true;
    [SerializeField] bool fitHeight = true;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        float worldHeight = Camera.main.orthographicSize * 2f;
        float worldWidth = worldHeight * Camera.main.aspect;

        float spriteHeight = sr.sprite.bounds.size.y * transform.localScale.y;
        float spriteWidth = sr.sprite.bounds.size.x * transform.localScale.x;

        float scaleX = transform.localScale.x;
        float scaleY = transform.localScale.y;

        if (fitHeight)
            scaleY = worldHeight / (sr.sprite.bounds.size.y);

        if (fitWidth)
            scaleX = worldWidth / (sr.sprite.bounds.size.x);

        // Use larger scale to ensure full coverage — no gaps
        float finalScale = Mathf.Max(scaleX, scaleY);

        transform.localScale = new Vector3(
            finalScale,
            finalScale,
            transform.localScale.z);

        Debug.Log(gameObject.name + " fitted scale: " + finalScale);
    }
}