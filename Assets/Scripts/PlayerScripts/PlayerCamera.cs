using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;          // ���� ������ ����� ������������
    public Vector3 offset = new Vector3(0, 8, -8); // ��������
    public float smoothSpeed = 5f;    // ��������� ����������

    void LateUpdate()
    {
        if (target == null) return;

        // �������� ������� ������
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // ������� �������� (������������)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        // ������ ������ ������� �� ������
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
