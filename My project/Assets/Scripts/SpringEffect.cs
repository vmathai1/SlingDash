using UnityEngine;

public class SpringEffect : MonoBehaviour
{
    Vector3 originalScale;
    bool isCompressed = false;
    float compressTimer = 0f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isCompressed)
        {
            compressTimer += Time.deltaTime;

            // Compress for 0.1s then spring back
            if (compressTimer < 0.1f)
                transform.localScale = new Vector3(
                    originalScale.x * 1.3f,
                    originalScale.y * 0.5f,
                    originalScale.z);
            else
            {
                transform.localScale = originalScale;
                isCompressed = false;
                compressTimer = 0f;
            }
        }
    }

    public void Compress()
    {
        isCompressed = true;
        compressTimer = 0f;
    }
}