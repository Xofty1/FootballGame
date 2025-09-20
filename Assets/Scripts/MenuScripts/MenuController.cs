using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void SelectPlayers(int count)
    {
        PlayerPrefs.SetInt("PlayersCount", count);
        SceneManager.LoadScene("GamePlayScene");
    }
}
    