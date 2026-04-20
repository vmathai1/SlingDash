using UnityEngine;

public class GridBackground : MonoBehaviour
{
    [Header("Grid Settings")]
    public Color lineColor = new Color(0.1f, 0.3f, 0.6f, 0.4f);
    public float lineSpacing = 1.5f;
    public int lineCount = 20;

    LineRenderer[] hLines;
    LineRenderer[] vLines;

    float scrollOffset = 0f;

    void Start()
    {
        DrawGrid();
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver()) return;

        float speed = GameManager.Instance.GetCurrentSpeed();
        scrollOffset += speed * Time.deltaTime;

        if (scrollOffset >= lineSpacing)
            scrollOffset -= lineSpacing;

        UpdateVerticalLines();
    }

    void DrawGrid()
    {
        // Horizontal lines
        int hCount = 8;
        hLines = new LineRenderer[hCount];
        for (int i = 0; i < hCount; i++)
        {
            GameObject go = new GameObject($"HLine_{i}");
            go.transform.parent = transform;
            LineRenderer lr = go.AddComponent<LineRenderer>();
            SetupLine(lr);
            float y = -4f + i * 1.5f;
            lr.SetPosition(0, new Vector3(-20f, y, 0.1f));
            lr.SetPosition(1, new Vector3(20f, y, 0.1f));
            hLines[i] = lr;
        }

        // Vertical lines
        vLines = new LineRenderer[lineCount];
        for (int i = 0; i < lineCount; i++)
        {
            GameObject go = new GameObject($"VLine_{i}");
            go.transform.parent = transform;
            LineRenderer lr = go.AddComponent<LineRenderer>();
            SetupLine(lr);
            vLines[i] = lr;
        }

        UpdateVerticalLines();
    }

    void UpdateVerticalLines()
    {
        for (int i = 0; i < vLines.Length; i++)
        {
            float x = -15f + i * lineSpacing - scrollOffset;
            vLines[i].SetPosition(0, new Vector3(x, -6f, 0.1f));
            vLines[i].SetPosition(1, new Vector3(x, 6f, 0.1f));
        }
    }

    void SetupLine(LineRenderer lr)
    {
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.startWidth = 0.04f;
        lr.endWidth = 0.04f;
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.sortingOrder = -1;
    }
}