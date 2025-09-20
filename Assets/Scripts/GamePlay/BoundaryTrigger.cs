using UnityEngine;

public class BoundaryTrigger : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        // Проверяем, что объект покинул поле
        if (other.CompareTag("Ball") || other.CompareTag("Player"))
        {
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.HandleObjectExit(other.gameObject);
            }
        }
    }
}