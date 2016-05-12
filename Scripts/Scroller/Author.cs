using UnityEngine;
using System.Collections;
using Definitions;

public class Author : MonoBehaviour
{
    public enum PageType { Standard, Special, First }
    [Tooltip("Standard: shared dilemma.\nSpecial: separate dilemmas\nFirst: no reactions.")]
    public PageType pageType;

    [TextArea(4, 10)]
    [SerializeField]
    private string reaction_left, reaction_right, dilemma;
    [TextArea(4, 10)]
    [SerializeField]
    [Tooltip("If different from left")]
    private string dilemma_right;

    [SerializeField]
    private Scroller contentScroller;

    void Start()
    {
        if (!contentScroller)
            contentScroller = GetComponentInChildren<Scroller>();
        Debug.Assert(contentScroller, "Set Scroller reference!");
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

        contentScroller.content_text = content;
    }
}
