using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RectResizer : MonoBehaviour
{
    // Auto-resize
    [Header("Auto-resize")]
    public float minSize;
    public float maxSize, offset;
    [Range(1, 5)]
    public float expansionRate = 2.5f, colliderWait = 4;
    public float initWait;
    public float runFor = 10;
    public bool useScroller = false;

    private RectTransform self;
    private Scroller scroller;
    private Text contentText;

    void Start()
    {
        self = GetComponent<RectTransform>();
        scroller = GetComponentInChildren<Scroller>();

        StartCoroutine(DelayedCollider());

        if (!FindObjectOfType<Author>().showImage)
        {
            offset = 0;
            GetComponentInChildren<UnityEngine.UI.Image>().enabled = false;
        }

        StartCoroutine(Expand());
    }

    IEnumerator DelayedCollider()
    {
        Collider2D c = GetComponentInChildren<Collider2D>();
        c.enabled = false;

        float elapsed = 0;

        while (elapsed < colliderWait)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        c.enabled = true;
    }

    IEnumerator Expand()
    {
        yield return new WaitForSeconds(initWait);

        if (!useScroller)
        {
            contentText = GetComponentInChildren<Text>();
            maxSize =  (contentText.text.Length * 4.3f) + offset;
            if (maxSize < minSize) maxSize = minSize;
        }

        float elapsed = 0;

        while (elapsed <= runFor)
        {
            float newSize = useScroller ? scroller.charsPrinted * expansionRate : elapsed * expansionRate * 100;
            self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Clamp(newSize + offset, minSize, maxSize));
            yield return null;
            if (!GameManager.Instance.IsPaused)
                elapsed += Time.deltaTime;
        }
    }
}
