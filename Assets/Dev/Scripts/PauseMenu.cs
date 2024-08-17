using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    private GameObject menu;
    private void Awake()
    {
        menu = transform.GetChild(0).gameObject;
        menu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;
        isPaused = true;
        menu.SetActive(true);
    }

    private void Resume()
    {
        Time.timeScale = 1;
        isPaused = false;
        menu.SetActive(false);
    }
}
