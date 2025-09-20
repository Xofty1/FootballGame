using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerId;
    public int teamId;
    public float rotationSpeed = 100f;
    public float moveSpeed = 5f;

    private int rotateDirection = 1;
    private Rigidbody rb;
    private GameManager gameManager;


    void Start()
    {
        rb = GetComponent<Rigidbody>();


        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager не найден в сцене!");
        }
    }

    void Update()
    {
        string button = "Fire" + playerId;

        if (Input.GetButtonDown(button))
            rotateDirection *= -1;

        

        if (Input.GetButton(button))
            rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.deltaTime);
        else
            transform.Rotate(Vector3.up * rotationSpeed * rotateDirection * Time.deltaTime);

        CheckForRespawnInput(button);
    }

    private void CheckForRespawnInput(string button)
    {
        print(button);
        if (Input.GetButton(button) && Input.GetKey(KeyCode.Alpha1))
        {
            if (gameManager != null)
            {
                gameManager.ResetPositionPlayer(playerId-1);
            }
        }
    }
}
