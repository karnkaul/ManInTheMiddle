using UnityEngine;
using System.Collections;

public class RectResizer : MonoBehaviour
{
    // Auto-resize
    [Header("Auto-resize")]
    public float minSize;
    public float maxSize, offset;
    [Range(1, 5)]
    public float expansionRate = 2.5f;

    private RectTransform self;
    private Scroller scroller;

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

    void Update()
    {
        self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Clamp((scroller.charsPrinted * expansionRate) + offset, minSize, maxSize));
    }
}
