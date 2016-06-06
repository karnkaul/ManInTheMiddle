using UnityEngine;
using System.Collections;

public class TitleButtons : MonoBehaviour
{
    public AudioClip alert;

    private AudioSource self;

    void Start()
    {
        self = GetComponent<AudioSource>();
    }

    public void Exit()
    {
        GameManager.Instance.Exit();
    }

    public void Load()
    {
        GameManager.Instance.LoadCheckpointScene();
    }

    public void Return()
    {
        transform.Find("Confirm Canvas").GetComponent<RectTransform>().localScale = Vector3.zero;
    }

    public void Restart()
    {
        if (FindObjectOfType<ContinueEnabler>().checkPointPresent)
        {
            transform.Find("Confirm Canvas").GetComponent<RectTransform>().localScale = Vector3.one;
            if (self && alert)
                self.PlayOneShot(alert);
        }
        else
            GameManager.Instance.ObliterateSaves();
    }
}
