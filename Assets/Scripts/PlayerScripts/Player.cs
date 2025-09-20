public record Player
{
    public int PlayerId { get; }
    public int TeamId { get; }
    public int GoalsScored { get; }
    public int Assists { get; }
    public float RotationSpeed { get; }
    public float MoveSpeed { get; }

    public Player(int playerId, int teamId, int goalsScored = 0, int assists = 0,
                 float rotationSpeed = 100f, float moveSpeed = 5f)
    {
        PlayerId = playerId;
        TeamId = teamId;
        GoalsScored = goalsScored;
        Assists = assists;
        RotationSpeed = rotationSpeed;
        MoveSpeed = moveSpeed;
    }
}