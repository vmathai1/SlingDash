using UnityEngine;
using UnityEngine.EventSystems;

public class BoostButton : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler
{
    public static bool IsPressed = false;
    public static float HoldTime = 0f;
    public static Vector2 ButtonScreenPos;
    public static float ButtonRadius = 100f;

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        HoldTime = 0f;
        ButtonScreenPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        HoldTime = 0f;
    }

    void Update()
    {
        if (IsPressed)
            HoldTime += Time.deltaTime;
    }
}