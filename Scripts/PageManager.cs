using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Definitions;

public class PageManager : MonoBehaviour
{
    public float contentWait = 1;
    public Scroller header, content;

    private bool contentFlushed = false;


    // DEBUGGING
    public bool _debug_delay = true;

	void Start ()
    {
        if (header && content)
            StartCoroutine(DisplayContent());
        GameManager.Instance.ToggleControls(false);
        GameManager.currentPM = this;
        Invoke("AutoEnableControls", GameManager.Instance.AutoEnableTimeout);
        FindObjectOfType<AdaptiveAuthor>().PopulateScroller(GameState.previousPage.playerChoice);

        if (_debug_delay)
        {
            foreach (Page page in GameState.pages)
                Debug.Log("Page:" + page.number + " | choice:" + page.playerChoice + "\n");
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
        current.playerChoice = playerChoice;
        GameState.previousPage = current;
        GameState.PushBackPreviousPage();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
