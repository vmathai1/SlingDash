using UnityEngine;

public class BGScroller : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform transform;
        public Transform duplicate;
        public float parallaxSpeed;
        public float gapAdjustment = 0f;
    }

    public ParallaxLayer[] layers;

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver()) return;

        float gameSpeed = GameManager.Instance.GetCurrentSpeed();

        foreach (var layer in layers)
        {
            if (layer.transform == null) continue;

            SpriteRenderer sr = 
                layer.transform.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            float width = sr.bounds.size.x + layer.gapAdjustment;
            float move = gameSpeed * layer.parallaxSpeed * Time.deltaTime;

            layer.transform.Translate(Vector2.left * move);
            if (layer.duplicate != null)
                layer.duplicate.Translate(Vector2.left * move);

            if (layer.transform.position.x <= -width)
            {
                if (layer.duplicate != null)
                    layer.transform.position = new Vector3(
                        layer.duplicate.position.x + width,
                        layer.transform.position.y,
                        layer.transform.position.z);
            }

            if (layer.duplicate != null &&
                layer.duplicate.position.x <= -width)
            {
                layer.duplicate.position = new Vector3(
                    layer.transform.position.x + width,
                    layer.duplicate.position.y,
                    layer.duplicate.position.z);
            }
        }
    }
}

