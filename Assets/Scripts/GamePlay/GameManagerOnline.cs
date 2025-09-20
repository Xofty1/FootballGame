using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManagerOnline : NetworkBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text winText;
    public Ball ball;

    private Transform[] spawnPoints;
    private Dictionary<ulong, int> playerTeams = new Dictionary<ulong, int>();

    private NetworkVariable<int> team1Score = new NetworkVariable<int>();
    private NetworkVariable<int> team2Score = new NetworkVariable<int>();

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            InitSpawnPoints();

        if (winText != null)
            winText.gameObject.SetActive(false);

        UpdateScoreUI();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
                AssignTeam(clientId);
        }

        team1Score.OnValueChanged += (_, __) => UpdateScoreUI();
        team2Score.OnValueChanged += (_, __) => UpdateScoreUI();
    }

    void AssignTeam(ulong clientId)
    {
        int teamId = (playerTeams.Count % 2) + 1;
        playerTeams[clientId] = teamId;

        var playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        if (playerObj != null)
        {
            var pc = playerObj.GetComponent<PlayerController>();
            pc.teamId = teamId;

            // Красим
            var rend = pc.GetComponentInChildren<Renderer>();
            if (rend != null)
                rend.material.color = teamId == 1 ? Color.red : Color.blue;

            // Спавн
            pc.transform.position = spawnPoints[teamId - 1].position;
        }
    }

    public void AddScore(int teamId, int scorerId)
    {
        if (!IsServer) return;

        if (teamId == 1) team1Score.Value++;
        else if (teamId == 2) team2Score.Value++;

        if (team1Score.Value >= 3)
            ShowWinClientRpc(1);
        else if (team2Score.Value >= 3)
            ShowWinClientRpc(2);
        else
            ResetBallClientRpc();
    }

    [ClientRpc]
    void ShowWinClientRpc(int teamId)
    {
        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            winText.text = $"Победила команда {teamId}!";
        }

        Time.timeScale = 0f;
        StartCoroutine(ReturnToMenuAfterDelay(5f));
    }

    private IEnumerator ReturnToMenuAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    [ClientRpc]
    void ResetBallClientRpc()
    {
        if (ball != null)
        {
            var rb = ball.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            ball.transform.position = Vector3.up * 0.5f;
            ball.lastTouchedPlayerId = -1;
            ball.lastTouchedTeamId = -1;
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Команда 1 {team1Score.Value} : {team2Score.Value} Команда 2";
    }

    void InitSpawnPoints()
    {
        spawnPoints = new Transform[2];
        spawnPoints[0] = new GameObject("Spawn1").transform;
        spawnPoints[0].position = new Vector3(-15, 0.5f, 13);

        spawnPoints[1] = new GameObject("Spawn2").transform;
        spawnPoints[1].position = new Vector3(15, 0.5f, -13);
    }
}
