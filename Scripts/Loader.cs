using UnityEngine;
using System.Collections;
using Definitions;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    // Async loading not working for mobile
    [SerializeField]
    private bool useAsync = false;

    [SerializeField]
    [Tooltip("Use this to branch to a different Line. Provide scene names")]
    private bool branch = false;
    [SerializeField]
    [Tooltip("Provide identical scene names for a Convergent page")]
    private string LineA, LineB;

    private bool preloadStarted = false, loadComplete = false, lastScene = false;
    public bool PreloadStarted { get { return preloadStarted; } }
    public bool LoadComplete { get { return loadComplete; } }
    public bool LastScene { get { return lastScene; } }

    private AsyncOperation ao;
    private int pageNumber;

    void Awake()
    {
        pageNumber = GetComponent<PageManager>().PageNumber;
    }

    public void LoadNext(Choice playerChoice)
    {
        Page current;
        //current.number = GameState.previousPage.number + 1;
        current.number = pageNumber;
        current.playerChoice = playerChoice;
        GameState.previousPage = current;

        // Don't push last scene
        //if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCount)
        GameState.PushBackPreviousPage();

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        StartCoroutine(Load());

    }

    public void StartPreloading()
    {
        if (!branch && useAsync)
            StartCoroutine(Preload());
        else
        {
            if (GameManager.Instance.DebugLevel >= DebugLevel.Notify)
            {
                if (branch)
                    Debug.Log("Branch detected.");
                else 
                    Debug.Log("Use Async disabled, skipping preloading.");
            }       
        }
    }

    IEnumerator Preload()
    {
        if (GameManager.Instance.DebugLevel >= DebugLevel.Notify)
            Debug.Log(name + pageNumber + " has started preloading.");

        ao = new AsyncOperation();
        int scene = SceneManager.GetActiveScene().buildIndex + 1;

        int sceneCount = SceneManager.sceneCountInBuildSettings;

        if (GameManager.Instance.DebugLevel >= DebugLevel.Verbose)
            Debug.Log("next scene:" + scene + " scenecount:" + sceneCount);

        if (scene < sceneCount)
        {
            ao = SceneManager.LoadSceneAsync(scene);
            ao.allowSceneActivation = false;

            // For Load()
            preloadStarted = true;
            loadComplete = false;

            while (!loadComplete)
            {
                // [0, 0.9] > [0, 1]
                float progress = Mathf.Clamp01(ao.progress / 0.9f);
                float prev_frame_progress = -1;

                if (GameManager.Instance.DebugLevel >= DebugLevel.Verbose && prev_frame_progress != progress)
                    Debug.Log("Loading progress: " + (progress * 100) + "%");

                prev_frame_progress = progress;

                // Loading completed
                if (ao.progress == 0.9f)
                    loadComplete = true;

                yield return null;
            }

            if (GameManager.Instance.DebugLevel >= DebugLevel.Notify)
                Debug.Log(name + pageNumber + " has finished preloading.");
        }

        else
        {
            lastScene = true;
            if (GameManager.Instance.DebugLevel >= DebugLevel.Notify)
                Debug.Log("Preload Coroutine: This is the last scene.");
        }
    }

    public IEnumerator Load()
    {
        if (!lastScene)
        {
            if (branch)
            {
                string lineToLoad = (GameState.previousPage.playerChoice == Choice.Left) ? LineA : LineB;
                SceneManager.LoadScene(lineToLoad);
            }

            else if (preloadStarted)
            {
                while (!loadComplete)
                    yield return null;
                ao.allowSceneActivation = true;
            }

            else
            {
                if (GameManager.Instance.DebugLevel >= DebugLevel.Notify)
                    Debug.Log("Scene not pre-loaded, forcing.");

                // Too messy to restart Load() after Preload() completes
                //StartCoroutine(Preload());
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        else
        {
            if (GameManager.Instance.DebugLevel >= DebugLevel.Notify)
                Debug.Log("No more scenes to load.");
        }
    }
}
