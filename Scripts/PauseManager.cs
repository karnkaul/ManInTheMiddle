using UnityEngine;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    private bool isEnabled = false;
    public bool IsEnabled { get { return isEnabled; } }

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
        GetComponent<RectTransform>().localScale = toggle ? Vector3.one : Vector3.zero;
    }
}
