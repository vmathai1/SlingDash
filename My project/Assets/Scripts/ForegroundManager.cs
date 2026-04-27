using UnityEngine;

public class ForegroundManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject foregroundTopPrefab;
    public GameObject foregroundBottomPrefab;

    [Header("Settings")]
    public float topY = 4f;
    public float bottomY = -4.5f;
    public float speedMultiplier = 1f;
    public int copyCount = 4;
    public float overlapAmount = 0.5f; // increase if gap still shows

    float topWidth;
    float bottomWidth;

    GameObject[] topCopies;
    GameObject[] bottomCopies;

    void Start()
    {
        SpriteRenderer topSR = foregroundTopPrefab.GetComponent<SpriteRenderer>();
        SpriteRenderer botSR = foregroundBottomPrefab.GetComponent<SpriteRenderer>();

        topWidth = topSR.bounds.size.x - overlapAmount;
        bottomWidth = botSR.bounds.size.x - overlapAmount;

        topCopies = new GameObject[copyCount];
        bottomCopies = new GameObject[copyCount];

        for (int i = 0; i < copyCount; i++)
        {
            // Pre-position all copies ahead including negative start
            float topX = -topWidth + (i * topWidth);
            topCopies[i] = Instantiate(foregroundTopPrefab,
                new Vector3(topX, topY, 0), Quaternion.identity);

            float bottomX = -bottomWidth + (i * bottomWidth);
            bottomCopies[i] = Instantiate(foregroundBottomPrefab,
                new Vector3(bottomX, bottomY, 0), Quaternion.identity);
        }
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        float speed = GameManager.Instance.GetCurrentSpeed() * speedMultiplier;
        float move = speed * Time.deltaTime;

        float camLeft = Camera.main.transform.position.x
                        - Camera.main.orthographicSize * Camera.main.aspect;

        // Top copies
        foreach (GameObject top in topCopies)
        {
            top.transform.Translate(Vector2.left * move);

            float rightEdge = top.transform.position.x + topWidth * 0.5f;
            if (rightEdge < camLeft)
            {
                float maxX = GetRightmostEdge(topCopies, topWidth);
                top.transform.position = new Vector3(
                    maxX + topWidth * 0.5f,
                    topY, 0);
            }
        }

        // Bottom copies
        foreach (GameObject bottom in bottomCopies)
        {
            bottom.transform.Translate(Vector2.left * move);

            float rightEdge = bottom.transform.position.x + bottomWidth * 0.5f;
            if (rightEdge < camLeft)
            {
                float maxX = GetRightmostEdge(bottomCopies, bottomWidth);
                bottom.transform.position = new Vector3(
                    maxX + bottomWidth * 0.5f,
                    bottomY, 0);
            }
        }
    }

    float GetRightmostEdge(GameObject[] copies, float width)
    {
        float maxX = float.MinValue;
        foreach (GameObject go in copies)
        {
            if (go == null) continue;
            float rightEdge = go.transform.position.x + width * 0.5f;
            if (rightEdge > maxX) maxX = rightEdge;
        }
        return maxX;
    }
}