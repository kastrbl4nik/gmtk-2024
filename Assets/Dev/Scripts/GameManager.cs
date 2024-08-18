using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private Animator animator;
    private readonly float transitionTime = 1f;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        animator = GetComponentInChildren<Animator>();
    }
    public void LoadScene(int index)
    {
        StartCoroutine(LoadSceneWithTransition(index));
    }

    public void LoadNextScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator LoadSceneWithTransition(int index)
    {
        animator.SetTrigger("End");
        yield return new WaitForSecondsRealtime(transitionTime);
        var asyncLoad = SceneManager.LoadSceneAsync(index);
        asyncLoad.completed += (operation) => animator.SetTrigger("Start");
    }
}
