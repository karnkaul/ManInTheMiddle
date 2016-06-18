using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleButtons : MonoBehaviour
{
    public AudioClip alert, click;

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
        StartCoroutine(_Load());
    }

    IEnumerator _Load()
    {
        if (self && click)
            self.PlayOneShot(click);
        yield return new WaitForSeconds(click.length);
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

    public void Credits()
    {
        StartCoroutine(_Credits());
    }

    IEnumerator _Credits()
    {
        if (self && alert)
        {
            self.PlayOneShot(click);
            yield return new WaitForSeconds(click.length);
        }
        yield return null;
        SceneManager.LoadScene("Credits");
    }
}
