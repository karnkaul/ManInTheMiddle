using UnityEngine;
using System.Collections;

public class ControlsEnabler : MonoBehaviour
{
    public Scroller content;

    private bool controlsEnabled = false;

    void Start ()
    {
        content = GetComponentInChildren<Scroller>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Enable controls if less than 50 chars remaining and scroll near bottom position.
        if (content.Remaining < 50 && !controlsEnabled)
        {
            if (GameManager.Instance.DebugLevel >= Definitions.DebugLevel.Notify)
                Debug.Log("Collider enabling controls.");
            controlsEnabled = GameManager.Instance.ToggleControls(true);
        }
    }
}
