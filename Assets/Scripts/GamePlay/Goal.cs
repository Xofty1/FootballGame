using UnityEngine;

public class Goal : MonoBehaviour
{
    public int[] defendingTeams;   // какие команды защищают эти ворота
    public GameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball != null)
        {
            // Проверка: если мяч забила команда, которая НЕ защищает эти ворота
            if (System.Array.IndexOf(defendingTeams, ball.lastTouchedTeamId) == -1 && ball.lastTouchedTeamId != -1)
            {
                manager.AddScore(ball.lastTouchedTeamId, ball.lastTouchedPlayerId);
            }
        }
    }
}
