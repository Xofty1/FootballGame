using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;          // кого камера будет преследовать
    public Vector3 offset = new Vector3(0, 8, -8); // смещение
    public float smoothSpeed = 5f;    // плавность следования

    void LateUpdate()
    {
        if (target == null) return;

        // Желаемая позиция камеры
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // Плавное движение (интерполяция)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        // Камера всегда смотрит на игрока
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
