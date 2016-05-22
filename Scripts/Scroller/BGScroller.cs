using UnityEngine;
using System.Collections;

public class BGScroller : MonoBehaviour
{
    [SerializeField]
    private float initWait = 1, shrinkSpeed = 1;
    [SerializeField]
    private Scroller contentScroller;

	IEnumerator Start ()
    {
        yield return new WaitForSeconds(initWait);
        StartCoroutine(Shrink());
	}

    IEnumerator Shrink()
    {
        while (transform.localScale.y > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - (shrinkSpeed * Time.deltaTime * contentScroller.CPF), 1);
            yield return null;
        }
    }
}
