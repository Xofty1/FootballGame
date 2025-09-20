using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : MonoBehaviour
{

    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        serverButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            PlayerPrefs.SetInt("PlayersCount", 1);
            SceneManager.LoadScene("GamePlayScene");
        });

        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            PlayerPrefs.SetInt("PlayersCount", 1);
            SceneManager.LoadScene("GamePlayScene");
        });

        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            PlayerPrefs.SetInt("PlayersCount", 1);
            SceneManager.LoadScene("GamePlayScene");
        });
    }
}