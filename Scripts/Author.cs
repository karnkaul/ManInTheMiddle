using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Definitions;

public class Author : MonoBehaviour
{
    public enum PageType { Standard, Special, First }
    [Tooltip("Standard: shared dilemma.\nSpecial: separate dilemmas\nFirst: no reactions.")]
    public PageType pageType;

    [Header("Content")]
    public bool scrollHeader = false;
    public bool showImage = true;
    public int pageNumber = 0;
    [SerializeField]
    [TextArea(2, 4)]
    private string header;
    [SerializeField]
    [TextArea(4, 10)]
    private string intro;
    [TextArea(4, 10)]
    [SerializeField]
    private string reaction_left, reaction_right, dilemma;
    [TextArea(4, 10)]
    [SerializeField]
    [Tooltip("If different from left")]
    private string dilemma_right;

    [SerializeField]
    private Scroller headerScroller, contentScroller;

    void Start()
    {
        if (!contentScroller)
            contentScroller = GameObject.Find("Content Text").GetComponentInChildren<Scroller>();
        if (!headerScroller)
            headerScroller = GameObject.Find("Header").GetComponent<Scroller>();
        Debug.Assert((contentScroller && headerScroller), "Set Scroller reference!");
    }

    public void PopulateScroller(Choice playerChoice)
    {
        // Header
        if (!scrollHeader)
            headerScroller.GetComponent<Text>().text = header;
        else
            headerScroller.content_text = header;

        // Content
        string content = (FindObjectOfType<Author>().showImage) ? "\n\n\n\n\n\n\n\n" : "";

        content += intro;

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

        contentScroller.content_text = content;
    }
}
