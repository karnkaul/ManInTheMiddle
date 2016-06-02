using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    private bool isEnabled = false;
    public bool IsEnabled { get { return isEnabled; } }

    public AudioMixerSnapshot paused, unpaused;

    void OnEnable()
    {
        GameManager.Pause += HandlePause;
    }

    void OnDisable()
    {
        GameManager.Pause -= HandlePause;
    }

    public void Mute()
    {
        //
    }

    public void Restart()
    {
        GameManager.Instance.ObliterateSaves();
    }
	
    public void Exit()
    {
        GameManager.Instance.Exit();
    }

    public void HandlePause(bool toggle)
    {
        if (toggle)
            paused.TransitionTo(0);
        else
            unpaused.TransitionTo(0);
        GetComponent<RectTransform>().localScale = toggle ? Vector3.one : Vector3.zero;
    }
}
