using UnityEngine;

public class CameraFit : MonoBehaviour
{
    void Start()
    {
        // Auto fit camera to screen
        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = 16f / 9f;

        Camera cam = GetComponent<Camera>();

        if (screenRatio >= targetRatio)
        {
            cam.orthographicSize = 5f;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            cam.orthographicSize = 5f * differenceInSize;
        }
    }
}