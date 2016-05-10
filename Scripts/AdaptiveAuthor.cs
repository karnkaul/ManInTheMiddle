using UnityEngine;
using System.Collections;
using Definitions;

public class AdaptiveAuthor : MonoBehaviour
{
    public enum PageType { Standard, Special, First }
    [Tooltip("Standard: shared dilemma.\nSpecial: separate dilemmas\nFirst: no reactions.")]
    public PageType pageType;

    [TextArea(4, 10)][SerializeField]
    private string reaction_left, reaction_right, dilemma;
    [TextArea(4, 10)][SerializeField][Tooltip("If different from left")]
    private string dilemma_right;

    // Auto-resize
    [Header("Auto-resize")]
    public float minSize;
    public float maxSize;
    private RectTransform self;
    private Scroller scroller;


	void Start ()
    {
        //PopulateScroller();
        self = GetComponent<RectTransform>();
        scroller = GetComponentInChildren<Scroller>();

        StartCoroutine(DelayedCollider());
	}

    public void PopulateScroller(Choice playerChoice)
    {
        string content = "";

        // First page won't contain reactions
        if (pageType != PageType.First)
        {
            content += (playerChoice == Choice.Left) ? reaction_left : reaction_right;
            //content += "\n\n";
        }

        // Special pages will have adaptive dilemmas
        if (pageType == PageType.Special)
            content += (playerChoice == Choice.Left) ? dilemma : dilemma_right;

        // First/Standard pages will have fixed dilemma
        else
            content += dilemma;

        GetComponentInChildren<Scroller>().content_text = content;
    }

    void Update ()
    {
        self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Clamp(scroller.charsPrinted * 3, minSize, maxSize));
	}

    IEnumerator DelayedCollider()
    {
        Collider2D c = GetComponentInChildren<Collider2D>();
        c.enabled = false;

        float elapsed = 0;

        while (elapsed < 2.0f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        c.enabled = true;
    }
}
