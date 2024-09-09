using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static bool GameFinished = false;

    public GameObject ResultPanel;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        StartSetter();
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowResults()
    {
        ResultPanel.SetActive(true);
    }

    void StartSetter()
    {
        ResultPanel.SetActive(false);
    }
}
