using UnityEngine;
using UnityEngine.EventSystems;

public class BoostButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static bool IsPressed = false;
    public static float HoldTime = 0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        HoldTime = 0f;
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