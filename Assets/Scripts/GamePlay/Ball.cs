using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector] public int lastTouchedPlayerId = -1;
    [HideInInspector] public int lastTouchedTeamId = -1;
    private Vector3 startPosition = new Vector3(0,-8,0);

    private void OnCollisionEnter(Collision collision)
    {
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player != null)
        {
            lastTouchedPlayerId = player.playerId;
            lastTouchedTeamId = player.teamId;
        }
    }
}
