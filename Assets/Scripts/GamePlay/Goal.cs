using UnityEngine;

public class Goal : MonoBehaviour
{
    public int[] defendingTeams;   // ����� ������� �������� ��� ������
    public GameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball != null)
        {
            // ��������: ���� ��� ������ �������, ������� �� �������� ��� ������
            if (System.Array.IndexOf(defendingTeams, ball.lastTouchedTeamId) == -1 && ball.lastTouchedTeamId != -1)
            {
                manager.AddScore(ball.lastTouchedTeamId, ball.lastTouchedPlayerId);
            }
        }
    }
}
