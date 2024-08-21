using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance;
    private Animator animator;
    private readonly float transitionTime = 1f;
    private void Awake()
    {

        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            return;
        }
        animator = GetComponentInChildren<Animator>();
    }
    public void LoadScene(int index)
    {
        if (Instance != this)
        {
            Instance.LoadScene(index);
            return;
        }
        StartCoroutine(LoadSceneWithTransition(index));
    }

    public void LoadNextScene()
    {
        if (Instance != this)
        {
            Instance.LoadNextScene();
            return;
        }
        LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadScene()
    {
        if (Instance != this)
        {
            Instance.ReloadScene();
            return;
        }
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator LoadSceneWithTransition(int index)
    {
        if (Instance != this)
        {
            yield return Instance.LoadSceneWithTransition(index);
        }
        animator.SetTrigger("End");
        yield return new WaitForSecondsRealtime(transitionTime);
        var asyncLoad = SceneManager.LoadSceneAsync(index);
        asyncLoad.completed += (operation) => animator.SetTrigger("Start");
    }
}
