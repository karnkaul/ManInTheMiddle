using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollPauseHandler : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.Pause += HandlePause;
    }

    void OnDisable()
    {
        GameManager.Pause -= HandlePause;
    }

    void HandlePause(bool toggle)
    {
        GetComponent<ScrollRect>().enabled = !toggle;
    }
}
