using UnityEngine;

public class GamePlayScene : MonoBehaviour
{
    void Start()
    {
        int playersCount = PlayerPrefs.GetInt("PlayersCount", 2);

        if (playersCount == 1)
        {
            GetComponent<GameManager>().enabled = false;
            GetComponent<GameManagerOnline>().enabled = true;
        }
        else
        {
            GetComponent<GameManager>().enabled = true;
            GetComponent<GameManagerOnline>().enabled = false;
        }
    }

}
