using UnityEngine;

public class FireEffect : MonoBehaviour
{
    [SerializeField] Sprite[] fireFrames;   // drag your 3 fire images here
    [SerializeField] float fireDuration = 0.4f;
    [SerializeField] float frameRate = 0.08f;

    SpriteRenderer sr;
    Transform tireTransform;

    float fireTimer = 0f;
    float frameTimer = 0f;
    int currentFrame = 0;
    bool isFiring = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        tireTransform = transform.parent;
        transform.SetParent(null);

        // Make sure fire is hidden at start
        sr.enabled = false;
    }

    void Update()
    {
        if (tireTransform == null) return;

        // Follow tire
        transform.position = new Vector3(
            tireTransform.position.x - 0.7f,
            tireTransform.position.y - 0.5f,
            tireTransform.position.z
        );

        transform.rotation = Quaternion.identity;

        if (!isFiring) return;

        // Count down fire duration
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            StopFire();
            return;
        }

        // Cycle through frames manually
        frameTimer -= Time.deltaTime;
        if (frameTimer <= 0f)
        {
            frameTimer = frameRate;
            currentFrame = (currentFrame + 1) % fireFrames.Length;
            sr.sprite = fireFrames[currentFrame];
        }
    }

public void TriggerFire()
    {
        Debug.Log("TriggerFire called! frames: " + fireFrames.Length);
        if (fireFrames == null || fireFrames.Length == 0) return;

        isFiring = true;
        fireTimer = fireDuration;
        frameTimer = frameRate;
        currentFrame = 0;
        sr.sprite = fireFrames[0];
        sr.enabled = true;
    }

    void StopFire()
    {
        isFiring = false;
        sr.enabled = false;
    }
}