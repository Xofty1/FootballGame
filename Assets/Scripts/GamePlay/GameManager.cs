using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;

    private Transform[] spawnPoints;
    public Goal[] goals;
    public TMP_Text scoreText;
    public TMP_Text winText;

    private int playersCount;
    private int teamsCount = 2;
    private Dictionary<int, int> teamScores = new Dictionary<int, int>();
    private List<PlayerController> players = new List<PlayerController>();
    private Ball ball;
    private Dictionary<int, string> playerControls = new Dictionary<int, string>()
{
    { 1, "Z" }, // Fire1
    { 2, "M" }, // Fire2
    { 3, "E" }, // Fire3
    { 4, "P" }  // Fire4
};

    void Start()
    {
        playersCount = PlayerPrefs.GetInt("PlayersCount", 2);
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            InitializeSpawnPoints();
        }
        SpawnPlayers(playersCount);

        ball = FindFirstObjectByType<Ball>();

        for (int i = 1; i <= teamsCount; i++)
            teamScores[i] = 0;

        if (winText != null)
            winText.gameObject.SetActive(false);

        UpdateScoreUI();
        
    }

    void ShowWin(int teamId)
    {
        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            winText.text = $"Победила команда {teamId}!";
        }

        // Останавливаем игру (можно по-разному, вот простой способ)
        Time.timeScale = 0f;

        // Если нужно, можно через 5 сек. вернуть в меню:
         StartCoroutine(ReturnToMenuAfterDelay(5f));
    }

    private IEnumerator ReturnToMenuAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    private void Update()
    {
        GoToMenu();
        CheckBallPosition();
    }
    void CheckBallPosition()
    {
        if (ball != null && ball.transform.position.y < -10f)
        {
            ResetBall();
        }
    }
    void GoToMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    void SpawnPlayers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject player = Instantiate(playerPrefab, spawnPoints[i].position, Quaternion.identity);

            int teamId = (i % teamsCount) + 1;

            PlayerController pc = player.GetComponent<PlayerController>();
            pc.playerId = i + 1;
            pc.teamId = teamId;
            players.Add(pc);

            // === Покраска игрока по команде ===
            Renderer rend = player.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                if (teamId == 1)
                    rend.material.color = Color.red;      // Команда 1 = красные
                else if (teamId == 2)
                    rend.material.color = Color.blue;     // Команда 2 = синие
            }

            // Камера игрока
            Camera cam = player.GetComponentInChildren<Camera>();
            switch (count)
            {
                case 2:
                    cam.rect = new Rect(0, i == 0 ? 0.5f : 0, 1, 0.5f);
                    break;
                case 3:
                    if (i == 1) // Первый игрок (i=0) получает верхнюю половину
                    {
                        cam.rect = new Rect(0, 0.5f, 1, 0.5f);
                        players[i].moveSpeed = 8f; // если нужно ускорить
                    }
                    else if (i == 0) // Второй игрок (i=1) - левая нижняя четверть
                    {
                        cam.rect = new Rect(0, 0, 0.5f, 0.5f);
                    }
                    else // i == 2 - Третий игрок (i=2) - правая нижняя четверть
                    {
                        cam.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                    }
                    break;
                case 4:
                    cam.rect = new Rect((i % 2) * 0.5f, (i / 2) * 0.5f, 0.5f, 0.5f);
                    break;
            }

            // UI подсказка
            CreatePlayerUI(cam, pc.playerId);
        }
    }

    public void HandleObjectExit(GameObject exitedObject)
    {
        if (exitedObject.CompareTag("Ball"))
        {
            ResetBall();
        }
        else if (exitedObject.CompareTag("Player"))
        {
            PlayerController player = exitedObject.GetComponent<PlayerController>();
            if (player != null)
            {
                ResetPositionPlayer(player.playerId-1);
            }
        }
    }
    void CreatePlayerUI(Camera cam, int playerId)
    {
        GameObject canvasGO = new GameObject($"Canvas_Player{playerId}");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
        canvas.planeDistance = 1;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject textGO = new GameObject("ControlHint");
        textGO.transform.SetParent(canvasGO.transform, false);

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        string buttonName = playerControls[playerId];
        tmp.text = $"Тыкай {buttonName}";
        tmp.fontSize = 60;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.yellow;

        if (tmp.font == null)
            tmp.font = TMP_Settings.defaultFontAsset;

        RectTransform rect = tmp.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.pivot = new Vector2(0.5f, 0);
        rect.anchoredPosition = new Vector2(0, 80);
        rect.sizeDelta = new Vector2(800, 150);
    }
    public void AddScore(int teamId, int scorerId)
    {
        if (!teamScores.ContainsKey(teamId))
            teamScores[teamId] = 0;

        teamScores[teamId]++;

        Debug.Log($"Гол! Игрок {scorerId} забил за команду {teamId}");

        UpdateScoreUI();

        // === Проверка на победу ===
        if (teamScores[teamId] >= 1)
        {
            ShowWin(teamId);
        }
        else
        {
            ResetPositions();
        }
    }

    void UpdateScoreUI()
    {
        scoreText.text = $"Команда 1 {teamScores[1]} : {teamScores[2]} Команда 2";
    }

    void ResetBall()
    {
        if (ball != null)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            ball.transform.position = Vector3.up * 0.5f;
            ball.lastTouchedPlayerId = -1;
            ball.lastTouchedTeamId = -1;
        }
    }

    void ResetPositions()
    {
        if (ball != null)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            ball.transform.position = Vector3.up * 0.5f;
            ball.lastTouchedPlayerId = -1;
            ball.lastTouchedTeamId = -1;
        }

        for (int i = 0; i < players.Count; i++)
        {
            Transform spawn = spawnPoints[i];
            PlayerController pc = players[i];
            pc.transform.position = spawn.position;
            pc.transform.rotation = spawn.rotation;

            Rigidbody rb = pc.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void ResetPositionPlayer(int i)
    {
        Transform spawn = spawnPoints[i];
        PlayerController pc = players[i];
        pc.transform.position = spawn.position;
        pc.transform.rotation = spawn.rotation;

        Rigidbody rb = pc.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void InitializeSpawnPoints()
    {
        spawnPoints = new Transform[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject spawnPoint = new GameObject($"SpawnPoint_{i + 1}");
            spawnPoints[i] = spawnPoint.transform;
        }

        spawnPoints[0].position = new Vector3(-15, 0.5f, 13);
        spawnPoints[1].position = new Vector3(15, 0.5f, -13);
        spawnPoints[2].position = new Vector3(-15, 0.5f, -13);
        spawnPoints[3].position = new Vector3(15, 0.5f, 13);
    }
}
