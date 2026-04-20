using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothY = 0.12f;
    [SerializeField] float smoothX = 0.08f;
    [SerializeField] float offsetY = 1.5f;
    [SerializeField] float offsetX = -1.5f; // negative = tire sits left of center

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float targetY = Mathf.Lerp(
            transform.position.y,
            target.position.y + offsetY,
            smoothY);

        float targetX = Mathf.Lerp(
            transform.position.x,
            target.position.x + offsetX,
            smoothX);

        transform.position = new Vector3(
            targetX,
            targetY,
            startPos.z
        );
    }
}