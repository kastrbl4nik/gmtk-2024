using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void LoadLevel(int index)
    {
        FindObjectOfType<GameManager>().LoadScene(index);
    }

    public void NewGame()
    {
        FindObjectOfType<GameManager>().LoadScene(1);
    }
}
