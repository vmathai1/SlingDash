using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothY = 0.12f;
    [SerializeField] float offsetY = 1.5f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float targetY = Mathf.Lerp(transform.position.y,
                        target.position.y + offsetY, smoothY);

        transform.position = new Vector3(
            startPos.x,
            targetY,
            startPos.z
        );
    }
}