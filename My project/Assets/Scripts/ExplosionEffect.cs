using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] Sprite[] explosionFrames;
    [SerializeField] float frameRate = 0.06f;

    SpriteRenderer sr;
    float frameTimer;
    int currentFrame;
    bool isPlaying;

   void Start()
{
    sr = GetComponent<SpriteRenderer>();
    if (sr == null)
        Debug.LogError("No SpriteRenderer on ExplosionSprite!");
    sr.enabled = false;
    Debug.Log("ExplosionEffect ready on: " + gameObject.name);
}

    void Update()
    {
        if (!isPlaying) return;

        frameTimer -= Time.deltaTime;
        if (frameTimer <= 0f)
        {
            frameTimer = frameRate;
            currentFrame++;

            if (currentFrame >= explosionFrames.Length)
            {
                isPlaying = false;
                sr.enabled = false;
                return;
            }

            sr.sprite = explosionFrames[currentFrame];
        }
    }

public void Play()
{
    if (explosionFrames == null || explosionFrames.Length == 0)
    {
        Debug.LogError("No explosion frames assigned!");
        return;
    }
    Debug.Log("Explosion playing! Frames: " + explosionFrames.Length);
    sr.enabled = true;
    isPlaying = true;
    currentFrame = 0;
    frameTimer = frameRate;
    sr.sprite = explosionFrames[0];
}
}