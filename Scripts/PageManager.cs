using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Definitions;

public class PageManager : MonoBehaviour
{
    public float contentWait = 1;
    public Scroller header, content;

    private bool contentFlushed = false;
    private AsyncOperation ao;


    // DEBUGGING
    public bool _debug_delay = true;

	void Start ()
    {
        if (header && content)
            StartCoroutine(DisplayContent());
        
        // Initialise
        if (SceneManager.GetActiveScene().buildIndex == 0)
            GameState.Reset();

        GameManager.Instance.ToggleControls(false);
        GameManager.currentPM = this;
        Invoke("AutoEnableControls", GameManager.Instance.AutoEnableTimeout);
        FindObjectOfType<AdaptiveAuthor>().PopulateScroller(GameState.previousPage.playerChoice);

        if (_debug_delay)
        {
            foreach (Page page in GameState.pages)
                Debug.Log("Page:" + page.number + " | choice:" + page.playerChoice + "\n");
        }

        // Preload 
        StartCoroutine(Preload());
    }

    void AutoEnableControls()
    {
        GameManager.Instance.ToggleControls(true);
    }
	
	IEnumerator DisplayContent (Definitions.Void Callback = null)
    {
        if (_debug_delay)
            yield return new WaitForSeconds(2);

        header.FlushText();
        while (!header.Completed)
            yield return null;

        yield return new WaitForSeconds(contentWait);
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
        current.number = GameState.previousPage.number + 1;
        Debug.Log("number:" + current.number);
        current.playerChoice = playerChoice;
        GameState.previousPage = current;

        // Don't push last scene
        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCount-1)
            GameState.PushBackPreviousPage();

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        if (!ao.isDone)
            ao.allowSceneActivation = true;
    }

    IEnumerator Preload()
    {
        ao = new AsyncOperation();
        int scene = SceneManager.GetActiveScene().buildIndex + 1;
        if (scene > SceneManager.sceneCount)
            scene = 0;

        ao = SceneManager.LoadSceneAsync(scene);
        ao.allowSceneActivation = false;

        bool loadComplete = false;

        while (!loadComplete)
        {
            // [0, 0.9] > [0, 1]
            float progress = Mathf.Clamp01(ao.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            // Loading completed
            if (ao.progress == 0.9f)
            {
                Debug.Log("Ready to load");
                //if (Input.anyKeyDown)
                //    ao.allowSceneActivation = true;
                loadComplete = true;
            }

            yield return null;
        }
    }
}
