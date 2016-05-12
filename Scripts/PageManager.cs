using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Definitions;

public class PageManager : MonoBehaviour
{
    [Header("Scene")]
    public int pageNumber;
    public bool checkpoint = false;

    [Header("Content")]
    public float contentWait = 1;
    public Scroller header;
    public Scroller content;
    public bool scrollHeader = true;

    [Header("Music")]
    public bool autoSwapMusic = false;
    public AudioClip swapFile;

    private bool contentFlushed = false, preloadStarted = false, loadComplete = false, lastScene = false;
    private AsyncOperation ao;


    // DEBUGGING
    public bool _debug_delay = true;

    void Awake()
    {
        if (checkpoint)
            Persistor.Save(SceneManager.GetActiveScene().name);
    }

	void Start ()
    {
        if (pageNumber == 0)
            Debug.Assert(false, "Check scene number");

        if (header && content)
            StartCoroutine(DisplayContent());

        // Initialise : messing with Persistor. Using manual reset for now (GameManager)
        //if (SceneManager.GetActiveScene().buildIndex == 0)
        //if (pageNumber == 1)
        //    GameState.Reset();

        GameManager.Instance.ToggleControls(false);
        GameManager.currentPM = this;

        Invoke("AutoEnableControls", GameManager.Instance.AutoEnableTimeout);
        //FindObjectOfType<AdaptiveAuthor>().PopulateScroller(GameState.previousPage.playerChoice);
        FindObjectOfType<Author>().PopulateScroller(GameState.previousPage.playerChoice);

        if (_debug_delay)
        {
            foreach (Page page in GameState.pages)
                Debug.Log("Page:" + page.number + " | choice:" + page.playerChoice + "\n");
        }

        // Music
        if (autoSwapMusic && swapFile != null)
            Invoke("SwapMusic", 1);

    }

    void SwapMusic()
    {
        MusicPlayer mp = FindObjectOfType<MusicPlayer>();
        if (mp)
        {
            mp.enqueue = swapFile;
            mp.Swap();

            if (GameManager.Instance.DebugLevel >= DebugLevel.Verbose)
                Debug.Log(swapFile.name + " swapped.");
        }
    }

    void AutoEnableControls()
    {
        GameManager.Instance.ToggleControls(true);
    }
	
	IEnumerator DisplayContent (Definitions.Void Callback = null)
    {
        if (_debug_delay)
            yield return new WaitForSeconds(2);

        if (scrollHeader)
        {
            header.FlushText();
            while (!header.Completed)
                yield return null;
            yield return new WaitForSeconds(contentWait);
        }

        content.FlushText();

        while (!content.Completed)
            yield return null;

        contentFlushed = true;

        if (Callback != null)
            Callback();
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
        StartCoroutine(Preload());
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

                if (GameManager.Instance.DebugLevel >= DebugLevel.Verbose)
                    Debug.Log("Loading progress: " + (progress * 100) + "%");

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

    IEnumerator Load()
    {
        if (!lastScene)
        {
            if (preloadStarted)
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
