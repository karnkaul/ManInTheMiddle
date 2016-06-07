using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Definitions;

public class PageManager : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField]
    private int pageNumber;
    public int PageNumber { get { return pageNumber; } }
    [SerializeField][Tooltip("Use with autoSwapMusic for seamless audio experience.")]
    private bool checkpoint = false;
    //[SerializeField] 
    //[Tooltip("Use this to branch to a different Line. Provide scene names.")]
    //private bool branch = false;
    //[SerializeField]
    //private string LineA, LineB;

    
    [Header("Content")]
    [SerializeField]
    private float contentWait = 1;
    public Scroller header;
    public Scroller content;

    [Header("Music")]
    [SerializeField]
    private bool autoSwapMusic = false;
    public AudioClip swapFile;

    private bool contentFlushed = false, preloadStarted = false, loadComplete = false, lastScene = false;
    private AsyncOperation ao;


    // DEBUGGING
    public bool _debug_delay = true;

    void Awake()
    {
        if (checkpoint)
            Persistor.Save(SceneManager.GetActiveScene().name);

        Author author = FindObjectOfType<Author>();
        if (author.pageNumber > 0)
            pageNumber = author.pageNumber;

        if (!author.showImage)
            contentWait = 0;
    }

	void Start ()
    {
        if (pageNumber == 0)
            Debug.Assert(false, "Check scene number");

        else if (pageNumber == -1)
            Debug.Log("Template scene.");

        FindObjectOfType<Author>().PopulateScroller(GameState.previousPage.playerChoice);

        if (header && content)
            StartCoroutine(DisplayContent());

        // Initialise : messing with Persistor. Using manual reset for now (GameManager)
        //if (SceneManager.GetActiveScene().buildIndex == 0)
        //if (pageNumber == 1)
        //    GameState.Reset();

        //GameManager.Instance.ToggleControls(false);
        //FindObjectOfType<ControlsManager>().Toggle(false);
        //if (GameManager.Instance.DebugLevel >= DebugLevel.Verbose)
        //    Debug.Log("Interface disabling controls.");
        GameManager.currentPM = this;

        Invoke("AutoEnableControls", GameManager.Instance.Autoenable);
        //FindObjectOfType<AdaptiveAuthor>().PopulateScroller(GameState.previousPage.playerChoice);

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
        GameManager.Instance.EnableControls();
        if (GameManager.Instance.DebugLevel >= DebugLevel.Verbose)
            Debug.Log("Auto enabling controls (" + GameManager.Instance.Autoenable + " s passed.");
    }
	
	IEnumerator DisplayContent (Definitions.Void Callback = null)
    {
        if (_debug_delay)
            yield return new WaitForSeconds(2);
        else
            yield return new WaitForSeconds(0.1f);

        if (FindObjectOfType<Author>().scrollHeader)
        {
            header.FlushText();
            while (!header.Completed)
                yield return null;
            yield return new WaitForSeconds(contentWait);
        }

        //content.FlushText();

        //while (!content.Completed)
        //    yield return null;

        //contentFlushed = true;

        yield return new WaitForSeconds(1);

        //if (GameManager.Instance.InterfaceControl)
        //{
        //    GameManager.Instance.ToggleControls(true);
        //    if (GameManager.Instance.DebugLevel >= DebugLevel.Verbose)
        //        Debug.Log("Text flushed, PM enabling controls.");
        //}

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
        Loader loader = GetComponent<Loader>();
        StartCoroutine(loader.Load());
        
    }

    public void StartPreloading()
    {
        GetComponent<Loader>().StartPreloading();
    }
}
